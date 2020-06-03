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
using System.Linq;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.ViewModels.Cards
{
  /// <summary>
  /// EditViewModel for <see cref="Deck"/>
  /// </summary>
  public sealed class DeckEditViewModel : EditViewModelBase<Deck>
  {
    private readonly Dictionary<string, CardTemplate> availableCardTemplates = new Dictionary<string, CardTemplate>();

    /// <summary>
    /// Id of the definition of the card
    /// </summary>
    public long CardTemplateId
    {
      get => Entity.DefaultCardTemplateId;
      set
      {
        if (CardTemplateId != value)
        {
          Entity.DefaultCardTemplateId = value;
          OnPropertyChanged();
        }
      }
    }

    /// <summary>
    /// Title of the deck
    /// </summary>
    public string DeckTitle
    {
      get => Entity?.Title;
      set
      {
        if (value != Entity.Title)
        {
          Entity.Title = value;
          OnPropertyChanged();
        }
      }
    }

    /// <summary>
    /// Property for <see cref="CardTemplateTitle"/>
    /// </summary>
    public PropertyProxy CardTemplateTitleProperty { get; private set; }

    /// <summary>
    /// Property for <see cref="Deck.Title"/>
    /// </summary>
    public PropertyProxy TitleProperty { get; private set; }

    /// <summary>
    /// Title of the card template
    /// </summary>
    public string CardTemplateTitle
    {
      get => availableCardTemplates.Values.SingleOrDefault(template => template.CardTemplateId == CardTemplateId)?.Title;
      set
      {
        if (CardTemplateTitle != value)
        {
          if (string.IsNullOrEmpty(value))
            CardTemplateId = default;
          else
            CardTemplateId = availableCardTemplates[value].CardTemplateId;
        }        
      }
    }

    /// <summary>
    /// The available card templates
    /// </summary>
    public List<string> AvailableCardTemplates => availableCardTemplates.Keys.ToList();

    /// <summary>
    /// Command for editing a card from the deck
    /// </summary>
    public NavigationCommand EditCardCommand { get; private set; }

    /// <summary>
    /// Command for deleting a card from the deck
    /// </summary>
    public EntityDeleteCommand<Card> DeleteCardCommand { get; private set; }

    /// <summary>
    /// Command for adding a new card to the deck
    /// </summary>
    public NavigationCommand NewCardCommand { get; private set; }

    /// <summary>
    /// Command for showing the practice statistics
    /// </summary>
    public NavigationCommand ShowStatisticsCommand { get; private set; }

    /// <summary>
    /// Command for practicing the deck
    /// </summary>
    public NavigationCommand PracticeDeckCommand { get; private set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="navigationManager">NavigationManager (Injected)</param>
    /// <param name="apiConnector">ApiConnector (Injected)</param>
    /// <param name="changeValidator">change validator (Injected)</param>
    public DeckEditViewModel(NavigationManager navigationManager, IApiConnector apiConnector,
      EntityChangeValidator<Deck> changeValidator)
      : base(navigationManager, apiConnector, changeValidator)
    { }

    ///<inheritdoc/>
    public override async Task<bool> InitializeAsync()
    {
      bool result = await LoadAvailaleCardTemplates();
      if (!result)
        return result;

      result = await base.InitializeAsync();
      if (!result)
        return result;

      InitializeCommands();

      CardTemplateTitleProperty = new PropertyProxy(
        () => CardTemplateTitle, 
        (value) => CardTemplateTitle = value,
        nameof(CardTemplateTitle), 
        Entity
        );
      RegisterPropertyProxy(CardTemplateTitleProperty);
      CardTemplateTitleProperty.Validator = (value, entity) => ValidateCardTemplateTitle(value);

      TitleProperty = new PropertyProxy(
        () => DeckTitle,
        (value) => DeckTitle = value,
        nameof(Entity.Title),
        Entity
      );
      RegisterPropertyProxy(TitleProperty);
      return true;
    }

    ///<inheritdoc/>
    protected override void CreateNewEntity()
    {
      Entity = new Deck();
      CardTemplateId = CardTemplate.DefaultCardTemplateId;
    }

    private void InitializeCommands()
    {
      PracticeDeckCommand = new NavigationCommand(NavigationManager)
      {
        CommandText = Messages.Practice,
        ToolTip = Messages.PracticeCommandToolTip.FormatWith(EntityNameHelper.GetName<Deck>()),
        IsRelative = true,
        IsEnabled = !IsNewEntity,
        TargetUri = "/Practice"
      };
      EditCardCommand = new NavigationCommand(NavigationManager)
      {
        CommandText = Components.Messages.Edit,
        ToolTip = Components.Messages.EditCommandToolTip.FormatWith(EntityNameHelper.GetName<Card>()),
        IsRelative = true,
        IsEnabled = !IsNewEntity,
        TargetUriFactory = (param) => $"/Cards/{(param as Card).Id}"
      };
      NewCardCommand = new NavigationCommand(NavigationManager)
      {
        CommandText = Components.Messages.New,
        ToolTip = Components.Messages.NewCommandToolTip.FormatWith(EntityNameHelper.GetName<Card>()),
        IsRelative = true,
        IsEnabled = !IsNewEntity,
        TargetUri = "/Cards/New"
      };
      ShowStatisticsCommand = new NavigationCommand(NavigationManager)
      {
        CommandText = Messages.PracticeStatistics,
        ToolTip = Messages.ShowStatisticsCommandToolTip.FormatWith(EntityNameHelper.GetName<Deck>()),
        IsRelative = true,
        IsEnabled = !IsNewEntity,
        TargetUri = "/Statistics"
      };

      DeleteCardCommand = new EntityDeleteCommand<Card>(ApiConnector)
      {
        IsEnabled = !IsNewEntity,
        CommandText = Components.Messages.Delete,
        ToolTip = Components.Messages.DeleteCommandToolTip.FormatWith(EntityNameHelper.GetName<Card>()),
        DeleteDialogTitle = Messages.DeleteCardDialogTitle,
        DeleteDialogTextFactory = (card) => Messages.DeleteCardDialogText.FormatWith(card.CardId)
      };
      DeleteCommand.DeleteDialogTitle = Messages.DeleteDeckDialogTitle;
      DeleteCommand.DeleteDialogText = Messages.DeleteDeckDialogText.FormatWith(Entity.Title);
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