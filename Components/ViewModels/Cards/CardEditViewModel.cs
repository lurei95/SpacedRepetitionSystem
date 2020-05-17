using Microsoft.AspNetCore.Components;
using SpacedRepetitionSystem.Components.Commands;
using SpacedRepetitionSystem.Components.Edits;
using SpacedRepetitionSystem.Components.Middleware;
using SpacedRepetitionSystem.Entities;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Entities.Validation.Core;
using SpacedRepetitionSystem.Utility.Dialogs;
using SpacedRepetitionSystem.Utility.Extensions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Components.ViewModels.Cards
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
    /// The fields of the card
    /// </summary>
    public List<CardField> Fields => Entity.Fields;

    /// <summary>
    /// The tags assigned to the the card
    /// </summary>
    public ObservableCollection<string> Tags { get; } = new ObservableCollection<string>();

    /// <summary>
    /// Command for showing the practice statistics
    /// </summary>
    public Command ShowStatisticsCommand { get; private set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="navigationManager">NavigationManager (Injected)</param>
    /// <param name="apiConnector">ApiConnector (Injected)</param>
    /// <param name="changeValidator">change validator (Injected)</param>
    public CardEditViewModel(NavigationManager navigationManager, IApiConnector apiConnector,
      EntityChangeValidator<Card> changeValidator) 
      : base(navigationManager, apiConnector, changeValidator)
    { 
      Tags.CollectionChanged += (sender, e) => OnPropertyChanged(nameof(Tags));
      ShowStatisticsCommand = new Command()
      {
        CommandText = Messages.PracticeStatistics,
        ExecuteAction = (param) => NavigationManager.NavigateTo(NavigationManager.Uri + "/Statistics/")
      };
    }

    ///<inheritdoc/>
    public override async Task InitializeAsync() 
    {
      foreach (CardTemplate cardTemplate in (await ApiConnector.GetAsync<CardTemplate>(new Dictionary<string, object>())).Result)
        availableCardTemplates.Add(cardTemplate.Title, cardTemplate);

      await base.InitializeAsync();
      CardTemplateId = Entity.CardTemplateId;
      Entity.DeckId = deckId;
      ChangeDeck();

      CardTemplateTitleProperty = new PropertyProxy(
       () => CardTemplateTitle,
       (value) => CardTemplateTitle = value,
        nameof(CardTemplateTitle),
        Entity
      );
      RegisterPropertyProperty(CardTemplateTitleProperty);

      CardTemplateTitleProperty.Validator = (value, entity) => ValidateCardTemplateTitle(value);
      ShowStatisticsCommand.IsEnabled = !IsNewEntity;
    }

    ///<inheritdoc/>
    protected override void CreateNewEntity()
    {
      Entity = new Card();
      CardTemplateId = CardTemplate.DefaultCardTemplateId; 
    }

    ///<inheritdoc/>
    protected override async Task<bool> SaveChanges()
    {
      bool success = await base.SaveChanges();
      if (success && IsNewEntity)
        NavigationManager.NavigateTo($"Decks/{DeckId}/Cards/New", true);
      return success;
    }

    ///<inheritdoc/>
    protected override async Task DeleteEntity()
    {
      ModalDialogManager.ShowDialog(Messages.DeleteCardDialogTitle, 
        Messages.DeleteCardDialogText.FormatWith(Entity.CardId), DialogButtons.YesNo, async (result) =>
      {
        if (result == DialogResult.Yes)
          await base.DeleteEntity();
      });
    }

    private async void ChangeDeck()
    {
      Deck deck = (await ApiConnector.GetAsync<Deck>(DeckId)).Result;
      if (IsNewEntity)
        CardTemplateId = deck.DefaultCardTemplateId;
      OnPropertyChanged();
    }

    private string ValidateCardTemplateTitle(string value)
      => string.IsNullOrEmpty(value) ? Errors.PropertyRequired.FormatWith(PropertyNames.CardTemplate) : null;
  }
}