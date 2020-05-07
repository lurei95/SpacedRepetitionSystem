using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using SpacedRepetitionSystem.Components.Commands;
using SpacedRepetitionSystem.Components.Edits;
using SpacedRepetitionSystem.Entities;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Entities.Validation.Core;
using SpacedRepetitionSystem.Logic.Controllers.Core;
using SpacedRepetitionSystem.Utility.Dialogs;
using SpacedRepetitionSystem.Utility.Extensions;
using SpacedRepetitionSystem.Utility.Notification;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Components.ViewModels.Cards
{
  /// <summary>
  /// EditViewModel for <see cref="Deck"/>
  /// </summary>
  public sealed class DeckEditViewModel : EditViewModelBase<Deck>
  {
    private readonly Dictionary<string, long> availableCardTemplates = new Dictionary<string, long>();
    private long? cardTemplateId;

    /// <summary>
    /// Id of the definition of the card
    /// </summary>
    public long? CardTemplateId
    {
      get => cardTemplateId;
      set
      {
        if (cardTemplateId != value)
        {
          cardTemplateId = value;
          if (CardTemplateId.HasValue)
          {
            Entity.DefaultCardTemplateId = value.Value;
            Entity.DefaultCardTemplate = ApiConnector.Get<CardTemplate>(value);
          }
          else
          {
            Entity.DefaultCardTemplateId = default;
            Entity.DefaultCardTemplate = null;
          }
          OnPropertyChanged();
        }
      }
    }

    /// <summary>
    /// Property for <see cref="CardTemplateTitle"/>
    /// </summary>
    public PropertyProxy CardTemplateTitleProperty { get; private set; }

    /// <summary>
    /// Property for <see cref="Entity.Title"/>
    /// </summary>
    public PropertyProxy TitleProperty { get; private set; }

    /// <summary>
    /// Title of the card template
    /// </summary>
    public string CardTemplateTitle
    {
      get => Entity?.DefaultCardTemplate?.Title;
      set
      {
        if (CardTemplateTitle != value)
          CardTemplateId = availableCardTemplates[value];
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
    /// Constructor
    /// </summary>
    /// <param name="context">DbContext (Injected)</param>
    /// <param name="navigationManager">NavigationManager (Injected)</param>
    /// <param name="apiConnector">ApiConnector (Injected)</param>
    /// <param name="changeValidator">change validator (Injected)</param>
    public DeckEditViewModel(DbContext context, NavigationManager navigationManager, IApiConnector apiConnector,
      EntityChangeValidator<Deck> changeValidator)
      : base(context, navigationManager, apiConnector, changeValidator)
    {
      EditCardCommand = new Command()
      {
        CommandText = Messages.Edit,
        ExecuteAction = (param) => EditCard(param as Card)
      };

      DeleteCardCommand = new Command()
      {
        CommandText = Messages.Delete,
        ExecuteAction = (param) => DeleteCard(param as Card)
      };

      NewCardCommand = new Command()
      {
        CommandText = Messages.New,
        ExecuteAction = (param) => NewCard()
      };
    }

    ///<inheritdoc/>
    public override async Task InitializeAsync()
    {
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

      await base.InitializeAsync();

      foreach (CardTemplate cardTemplate in await ApiConnector.Get<CardTemplate>(null))
        availableCardTemplates.Add(cardTemplate.Title, cardTemplate.CardTemplateId);
      CardTemplateTitleProperty.Validator = (value, entity) => ValidateCardTemplateTitle(value);
    }

    ///<inheritdoc/>
    protected override void DeleteEntity()
    {
      ModalDialogManager.ShowDialog(Messages.DeleteDeckDialogTitle, 
        Messages.DeleteDeckDialogText.FormatWith(Entity.Title), DialogButtons.YesNo, (result) =>
      {
        if (result == DialogResult.Yes)
          base.DeleteEntity();
      });
    }

    ///<inheritdoc/>
    protected override void CreateNewEntity()
    {
      Entity = new Deck();
      CardTemplateId = CardTemplate.DefaultCardTemplateId;
    }

    private void DeleteCard(Card card)
    {
      if (ApiConnector.Delete(card))
        NotificationMessageProvider.ShowSuccessMessage(Messages.EntityDeleted.FormatWith(card.GetDisplayName()));
    }

    private void EditCard(Card card)
    { NavigationManager.NavigateTo(NavigationManager.Uri + "/Cards/" + card.Id); }

    private void NewCard()
    { NavigationManager.NavigateTo(NavigationManager.Uri + "/Cards/New"); }

    private string ValidateCardTemplateTitle(string value)
      => string.IsNullOrEmpty(value) ? Errors.PropertyRequired.FormatWith(PropertyNames.CardTemplate) : null;
  }
}