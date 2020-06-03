using Microsoft.AspNetCore.Components;
using SpacedRepetitionSystem.Components.Commands;
using SpacedRepetitionSystem.Components.Edits;
using SpacedRepetitionSystem.Components.Middleware;
using SpacedRepetitionSystem.Components.ViewModels;
using SpacedRepetitionSystem.Entities;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Entities.Validation.Core;
using SpacedRepetitionSystem.Utility.Extensions;
using SpacedRepetitionSystem.Utility.Notification;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.ViewModels.Cards
{
  /// <summary>
  /// EditViewModel for a <see cref="Card"/>
  /// </summary>
  public sealed class CardEditViewModel : EditViewModelBase<Card>
  {
    private readonly Dictionary<string, CardTemplate> availableCardTemplates = new Dictionary<string, CardTemplate>();
    private long deckId;
    private long cardTemplateId;

    /// <summary>
    /// Id of the deck the card belongs to
    /// </summary>
    public long DeckId 
    {
      get => deckId;
      set
      {
        if (deckId != value)
        {
          deckId = value;
          if (Entity != null)
          {
            Entity.DeckId = value;
            ChangeDeck();
          }
        }
      }
    }

    /// <summary>
    /// Id of the definition of the card
    /// </summary>
    public long CardTemplateId
    {
      get => cardTemplateId;
      set
      {
        if (cardTemplateId != value)
        {
          cardTemplateId = value;
          Entity.CardTemplateId = value;
          CardTemplate template = availableCardTemplates.Values.Single(template => template.CardTemplateId == value);
          Entity.Fields.Clear();
          foreach (CardFieldDefinition fieldDefinition in template.FieldDefinitions)
            Entity.Fields.Add(new CardField()
            {
              FieldId = fieldDefinition.FieldId,
              CardFieldDefinition = fieldDefinition,
              CardId = Entity.CardId,
              CardTemplateId = value,
              FieldName = fieldDefinition.FieldName
            });
          OnPropertyChanged();
        }
      }
    }

    /// <summary>
    /// Property for <see cref="CardTemplateTitle"/>
    /// </summary>
    public PropertyProxy CardTemplateTitleProperty { get; private set; }


    /// <summary>
    /// Title of the card template
    /// </summary>
    public string CardTemplateTitle
    {
      get => availableCardTemplates.Values.SingleOrDefault(template => template.CardTemplateId == CardTemplateId)?.Title;
      set
      {
        if (CardTemplateTitle != value)
          CardTemplateId = availableCardTemplates[value].CardTemplateId;
      }
    }

    /// <summary>
    /// The available card templates
    /// </summary>
    public List<string> AvailableCardTemplates => availableCardTemplates.Keys.ToList();

    /// <summary>
    /// The tags assigned to the the card
    /// </summary>
    public ObservableCollection<string> Tags { get; } = new ObservableCollection<string>();

    /// <summary>
    /// FieldName Properties
    /// </summary>
    public List<PropertyProxy> FieldValueProperties { get; } = new List<PropertyProxy>();

    /// <summary>
    /// Command for showing the practice statistics
    /// </summary>
    public NavigationCommand ShowStatisticsCommand { get; private set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="navigationManager">NavigationManager (Injected)</param>
    /// <param name="apiConnector">ApiConnector (Injected)</param>
    /// <param name="changeValidator">change validator (Injected)</param>
    public CardEditViewModel(NavigationManager navigationManager, IApiConnector apiConnector,
      EntityChangeValidator<Card> changeValidator) 
      : base(navigationManager, apiConnector, changeValidator)
    { Tags.CollectionChanged += (sender, e) => OnPropertyChanged(nameof(Tags)); }

    ///<inheritdoc/>
    public override async Task<bool> InitializeAsync() 
    {
      bool result = await LoadAvailaleCardTemplates();
      if (!result)
        return result;

      result = await base.InitializeAsync();
      if (!result)
        return false;

      if (!IsNewEntity && DeckId != Entity.DeckId)
      {
        NotificationMessageProvider.ShowErrorMessage(
          Components.Errors.EntityDoesNotExist.FormatWith(EntityNameHelper.GetName<Deck>(), DeckId));
        return false;
      }

      cardTemplateId = Entity.CardTemplateId;
      if (IsNewEntity)
      {
        Entity.DeckId = deckId;
        ChangeDeck();
      }

      InitializeCommands();

      RegisterPropertyProxies();
      CardTemplateTitleProperty.Validator = (value, entity) => ValidateCardTemplateTitle(value);
      return true;
    }

    ///<inheritdoc/>
    protected override void CreateNewEntity()
    {
      Entity = new Card();
      CardTemplateId = CardTemplate.DefaultCardTemplateId; 
    }

    private void InitializeCommands()
    {
      ShowStatisticsCommand = new NavigationCommand(NavigationManager)
      {
        CommandText = Messages.PracticeStatistics,
        ToolTip = Messages.ShowStatisticsCommandToolTip.FormatWith(EntityNameHelper.GetName<Card>()),
        IsRelative = true,
        TargetUri = "/Statistics"
      };

      DeleteCommand.DeleteDialogTitle = Messages.DeleteCardDialogTitle;
      DeleteCommand.DeleteDialogText = Messages.DeleteCardDialogText.FormatWith(Entity.CardId);
      ShowStatisticsCommand.IsEnabled = DeleteCommand.IsEnabled = !IsNewEntity;
    }

    ///<inheritdoc/>
    protected override void OnEntitySaved(Card entity)
    {
      if (IsNewEntity)
        NavigationManager.NavigateTo($"Decks/{DeckId}/Cards/New", true);
      else
        base.OnEntitySaved(entity);
    }

    private async void ChangeDeck()
    {
      Deck deck = (await ApiConnector.GetAsync<Deck>(DeckId)).Result;
      if (IsNewEntity)
        CardTemplateId = deck.DefaultCardTemplateId;
      OnPropertyChanged();
    }

    private void RegisterPropertyProxies()
    {
      CardTemplateTitleProperty = new PropertyProxy(
        () => CardTemplateTitle,
        (value) => CardTemplateTitle = value,
        nameof(CardTemplateTitle),
        Entity
      );
      RegisterPropertyProxy(CardTemplateTitleProperty);

      foreach (CardField field in Entity.Fields)
      {
        PropertyProxy proxy = new PropertyProxy(
          () => field.Value,
          (value) => field.Value = value,
          nameof(CardField.Value),
          field
        );
        RegisterPropertyProxy(proxy);
        FieldValueProperties.Add(proxy);
      }
    }

    private string ValidateCardTemplateTitle(string value)
      => string.IsNullOrEmpty(value) ? Errors.PropertyRequired.FormatWith(EntityNameHelper.GetName<CardTemplate>()) : null;

    private async Task<bool> LoadAvailaleCardTemplates()
    {
      ApiReply<List<CardTemplate>> reply = await ApiConnector.GetAsync<CardTemplate>(new Dictionary<string, object>());
      if (!reply.WasSuccessful)
      {
        NotificationMessageProvider.ShowErrorMessage(reply.ResultMessage);
        return false;
      }
      foreach (CardTemplate cardTemplate in reply.Result)
        availableCardTemplates.Add(cardTemplate.Title, cardTemplate);
      return true;
    }
  }
}