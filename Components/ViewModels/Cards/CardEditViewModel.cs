using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using SpacedRepetitionSystem.Components.Edits;
using SpacedRepetitionSystem.Entities;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Entities.Validation.Core;
using SpacedRepetitionSystem.Logic.Controllers.Core;
using SpacedRepetitionSystem.Utility.Dialogs;
using SpacedRepetitionSystem.Utility.Extensions;
using System;
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
            Entity.Deck = ApiConnector.Get<Deck>(DeckId);
            if (IsNewEntity)
            {
              CardTemplateId = Entity.Deck.DefaultCardTemplateId;
              Entity.CardTemplate = ApiConnector.Get<CardTemplate>(Entity.CardTemplateId);
            }
            OnPropertyChanged();
          }
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
      get => Entity?.CardTemplate?.Title;
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

      await base.InitializeAsync();

      foreach (CardTemplate cardTemplate in await ApiConnector.Get<CardTemplate>(null))
        availableCardTemplates.Add(cardTemplate.Title, cardTemplate.CardTemplateId);
      CardTemplateTitleProperty.Validator = (value, entity) => ValidateCardTemplateTitle(value);
    }

    ///<inheritdoc/>
    protected override void CreateNewEntity()
    {
      Entity = new Card { DueDate = DateTime.Today };
      CardTemplateId = CardTemplate.DefaultCardTemplateId;
    }

    ///<inheritdoc/>
    protected override bool SaveChanges()
    {
      bool success = base.SaveChanges();
      if (success && IsNewEntity)
        NavigationManager.NavigateTo($"Decks/{DeckId}/Cards/New", true);
      return success;
    }

    ///<inheritdoc/>
    protected override void DeleteEntity()
    {
      ModalDialogManager.ShowDialog(Messages.DeleteCardDialogTitle, 
        Messages.DeleteCardDialogText.FormatWith(Entity.CardId), DialogButtons.YesNo, (result) =>
      {
        if (result == DialogResult.Yes)
          base.DeleteEntity();
      });
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

    private string ValidateCardTemplateTitle(string value)
      => string.IsNullOrEmpty(value) ? Errors.PropertyRequired.FormatWith(PropertyNames.CardTemplate) : null;
  }
}