using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using SpacedRepetitionSystem.Components.Edits;
using SpacedRepetitionSystem.Entities;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Entities.Validation.Core;
using SpacedRepetitionSystem.Logic.Controllers.Core;
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
    private readonly Dictionary<string, long> availableDecks = new Dictionary<string, long>();
    private readonly Dictionary<string, long> availableCardTemplates = new Dictionary<string, long>();

    private long? deckId;
    private long? cardTemplateId;

    /// <summary>
    /// Id of the deck the card belongs to
    /// </summary>
    public long? DeckId 
    {
      get => deckId;
      set
      {
        if (deckId != value)
        {
          deckId = value;
          if (DeckId.HasValue)
          {
            Entity.DeckId = DeckId.Value;
            Entity.Deck = Context.Set<Deck>().Find(DeckId);
            CardTemplateId = Entity.Deck.DefaultCardTemplateId;
            Entity.CardTemplate = Context.Set<CardTemplate>().Find(Entity.CardTemplateId);
          }
          else
          {
            Entity.DeckId = default;
            Entity.Deck = null;
          }
          OnPropertyChanged(nameof(DeckTitle));
        }
      }
    }

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
            ChangeCardTemplate(CardTemplateId.Value);
          else
          {
            Entity.CardTemplateId = default;
            Entity.CardTemplate = null;
            Entity.Fields.Clear();
          }
          OnPropertyChanged(nameof(DeckTitle));
        }
      }
    }

    /// <summary>
    /// Property for <see cref="DeckTitle"/>
    /// </summary>
    public PropertyProxy CardTemplateTitleProperty { get; private set; }

    /// <summary>
    /// Property for <see cref="CardTemplateTitle"/>
    /// </summary>
    public PropertyProxy DeckTitleProperty { get; private set; }

    /// <summary>
    /// Title of the card template
    /// </summary>
    public string CardTemplateTitle
    {
      get => Entity?.CardTemplate?.Title;
      set
      {
        if (CardTemplateTitle != value)
          CardTemplateId = availableCardTemplates[value];
      }
    }

    /// <summary>
    /// Title of the deck
    /// </summary>
    public string DeckTitle
    {
      get => Entity?.Deck?.Title;
      set
      {
        if (DeckTitle != value)
          DeckId = availableDecks[value];
      }
    }

    /// <summary>
    /// The available decks
    /// </summary>
    public List<string> AvailableDecks => availableDecks.Keys.ToList();

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
    /// Constructor
    /// </summary>
    /// <param name="context">DbContext (Injected)</param>
    /// <param name="navigationManager">NavigationManager (Injected)</param>
    /// <param name="apiConnector">ApiConnector (Injected)</param>
    /// <param name="changeValidator">change validator (Injected)</param>
    public CardEditViewModel(DbContext context, NavigationManager navigationManager, IApiConnector apiConnector,
      EntityChangeValidator<Card> changeValidator) 
      : base(context, navigationManager, apiConnector, changeValidator)
    { Tags.CollectionChanged += (sender, e) => OnPropertyChanged(nameof(Tags)); }

    public override async Task InitializeAsync() 
    {
      CardTemplateTitleProperty = new PropertyProxy(
        () => DeckTitle,
        (value) => DeckTitle = value,
        nameof(DeckTitle)
      );
      DeckTitleProperty = new PropertyProxy(
        () => DeckTitle,
        (value) => DeckTitle = value,
        nameof(DeckTitle)
      );

      await base.InitializeAsync();

      foreach (Deck deck in await ApiConnector.Get<Deck>(null))
        availableDecks.Add(deck.Title, deck.DeckId);
      foreach (CardTemplate cardTemplate in await ApiConnector.Get<CardTemplate>(null))
        availableCardTemplates.Add(cardTemplate.Title, cardTemplate.CardTemplateId);

      DeckTitleProperty.Validator = (value) => ValidatePractiecSetTitle(value);
      CardTemplateTitleProperty.Validator = (value) => ValidateCardTemplateTitle(value);
    }

    ///<inheritdoc/>
    protected override void CreateNewEntity()
    {
      Entity = new Card();
      CardTemplateId = CardTemplate.DefaultCardTemplateId;
    }

    private void ChangeCardTemplate(long id)
    {
      Entity.CardTemplateId = id;
      Entity.CardTemplate = ApiConnector.Get<CardTemplate>(id);
      Entity.Fields.Clear();
      foreach (CardFieldDefinition fieldDefinition in Entity.CardTemplate.FieldDefinitions)
        Entity.Fields.Add(new CardField()
        {
          CardId = Entity.CardId,
          CardTemplateId = id,
          FieldName = fieldDefinition.FieldName
        });
    }

    private string ValidatePractiecSetTitle(string value) 
      => string.IsNullOrEmpty(value) ? Errors.PropertyRequired.FormatWith(PropertyNames.Deck) : null;

    private string ValidateCardTemplateTitle(string value)
      => string.IsNullOrEmpty(value) ? Errors.PropertyRequired.FormatWith(PropertyNames.CardTemplate) : null;
  }
}