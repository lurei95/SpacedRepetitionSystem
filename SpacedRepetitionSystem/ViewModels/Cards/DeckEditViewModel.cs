using Microsoft.AspNetCore.Components;
using SpacedRepetitionSystem.Components.Commands;
using SpacedRepetitionSystem.Components.Edits;
using SpacedRepetitionSystem.Components.Middleware;
using SpacedRepetitionSystem.Components.ViewModels;
using SpacedRepetitionSystem.Entities;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Entities.Validation.Core;
using SpacedRepetitionSystem.Utility.Dialogs;
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
          CardTemplateId = availableCardTemplates[value].CardTemplateId;
      }
    }

    /// <summary>
    /// The available card templates
    /// </summary>
    public List<string> AvailableCardTemplates => availableCardTemplates.Keys.ToList();

    /// <summary>
    /// Command for editing a card from the deck
    /// </summary>
    public Command EditCardCommand { get; private set; }

    /// <summary>
    /// Command for deleting a card from the deck
    /// </summary>
    public Command DeleteCardCommand { get; private set; }

    /// <summary>
    /// Command for adding a new card to the deck
    /// </summary>
    public Command NewCardCommand { get; private set; }

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
    public DeckEditViewModel(NavigationManager navigationManager, IApiConnector apiConnector,
      EntityChangeValidator<Deck> changeValidator)
      : base(navigationManager, apiConnector, changeValidator)
    {
      EditCardCommand = new Command()
      {
        CommandText = Components.Messages.Edit,
        ExecuteAction = (param) => EditCard(param as Card)
      };

      DeleteCardCommand = new Command()
      {
        CommandText = Components.Messages.Delete,
        ExecuteAction = async (param) => await DeleteCard(param as Card)
      };

      NewCardCommand = new Command()
      {
        CommandText = Components.Messages.New,
        ExecuteAction = (param) => NewCard()
      };

      ShowStatisticsCommand = new Command()
      {
        CommandText = Messages.PracticeStatistics,
        ExecuteAction = (param) => NavigationManager.NavigateTo(NavigationManager.Uri + "/Statistics/")
      };
    }

    ///<inheritdoc/>
    public override async Task<bool> InitializeAsync()
    {
      foreach (CardTemplate cardTemplate in (await ApiConnector.GetAsync<CardTemplate>(new Dictionary<string, object>())).Result)
        availableCardTemplates.Add(cardTemplate.Title, cardTemplate);

      bool result = await base.InitializeAsync();
      if (!result)
        return false;

      CardTemplateTitleProperty = new PropertyProxy(
        () => CardTemplateTitle,
        (value) => CardTemplateTitle = value,
        nameof(CardTemplateTitle),
        Entity
      );
      RegisterPropertyProperty(CardTemplateTitleProperty);
      TitleProperty = new PropertyProxy(
        () => Entity.Title,
        (value) => Entity.Title = value,
        nameof(Entity.Title),
        Entity
      );
      RegisterPropertyProperty(TitleProperty);
      CardTemplateTitleProperty.Validator = (value, entity) => ValidateCardTemplateTitle(value);
      return true;
    }

    ///<inheritdoc/>
    protected override async Task DeleteEntity()
    {
      ModalDialogManager.ShowDialog(Messages.DeleteDeckDialogTitle, 
        Messages.DeleteDeckDialogText.FormatWith(Entity.Title), DialogButtons.YesNo, async (result) =>
      {
        if (result == DialogResult.Yes)
          await base.DeleteEntity();
      });
      await Task.FromResult<object>(null);
    }

    ///<inheritdoc/>
    protected override void CreateNewEntity()
    {
      Entity = new Deck();
      CardTemplateId = CardTemplate.DefaultCardTemplateId;
    }

    private async Task DeleteCard(Card card)
    {
      ApiReply reply = await ApiConnector.DeleteAsync(card);
      if (reply.WasSuccessful)
        NotificationMessageProvider.ShowSuccessMessage(Components.Messages.EntityDeleted.FormatWith(card.GetDisplayName()));
      else
        NotificationMessageProvider.ShowErrorMessage(reply.ResultMessage);
    }

    private void EditCard(Card card)
    { NavigationManager.NavigateTo(NavigationManager.Uri + "/Cards/" + card.Id); }

    private void NewCard()
    { NavigationManager.NavigateTo(NavigationManager.Uri + "/Cards/New"); }

    private string ValidateCardTemplateTitle(string value)
      => string.IsNullOrEmpty(value) ? Errors.PropertyRequired.FormatWith(PropertyNames.CardTemplate) : null;
  }
}