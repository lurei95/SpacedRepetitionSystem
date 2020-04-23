using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using SpacedRepetitionSystem.Components.Edits;
using SpacedRepetitionSystem.Entities;
using SpacedRepetitionSystem.Entities.Entities.SmartCards;
using SpacedRepetitionSystem.Entities.Validation.Core;
using SpacedRepetitionSystem.Logic.Controllers.Core;
using SpacedRepetitionSystem.Utility.Extensions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SpacedRepetitionSystem.Components.ViewModels.SmartCards
{
  /// <summary>
  /// EditViewModel for a <see cref="SmartCard"/>
  /// </summary>
  public sealed class SmartCardEditViewModel : EditViewModelBase<SmartCard>
  {
    private readonly Dictionary<string, long> availableDecks = new Dictionary<string, long>();
    private readonly Dictionary<string, long> availableSmartCardDefinitions = new Dictionary<string, long>();

    private long? practiceSetId;
    private long? smartCardDefinitionId;

    /// <summary>
    /// Id of the Practice set the smart card belongs to
    /// </summary>
    public long? PracticeSetId 
    {
      get => practiceSetId;
      set
      {
        if (practiceSetId != value)
        {
          practiceSetId = value;
          if (PracticeSetId.HasValue)
          {
            Entity.PracticeSetId = PracticeSetId.Value;
            Entity.PracticeSet = Context.Set<PracticeSet>().Find(PracticeSetId);
            SmartCardDefinitionId = Entity.PracticeSet.DefaultSmartCardDefinitionId;
            Entity.SmartCardDefinition = Context.Set<SmartCardDefinition>().Find(Entity.SmartCardDefinitionId);
          }
          else
          {
            Entity.PracticeSetId = default;
            Entity.PracticeSet = null;
          }
          OnPropertyChanged(nameof(PracticeSetTitle));
        }
      }
    }

    /// <summary>
    /// Id of the definition of the smart card
    /// </summary>
    public long? SmartCardDefinitionId
    {
      get => smartCardDefinitionId;
      set
      {
        if (smartCardDefinitionId != value)
        {
          smartCardDefinitionId = value;
          if (SmartCardDefinitionId.HasValue)
            ChangeSmartCardDefinition(SmartCardDefinitionId.Value);
          else
          {
            Entity.SmartCardDefinitionId = default;
            Entity.SmartCardDefinition = null;
            Entity.Fields.Clear();
          }
          OnPropertyChanged(nameof(SmartCardDefinitionTitle));
        }
      }
    }

    /// <summary>
    /// Property for <see cref="SmartCardDefinitionTitle"/>
    /// </summary>
    public PropertyProxy SmartCardDefinitionTitleProperty { get; private set; }

    /// <summary>
    /// Property for <see cref="PracticeSetTitle"/>
    /// </summary>
    public PropertyProxy PracticeSetTitleProperty { get; private set; }

    /// <summary>
    /// Title of the smart card definition
    /// </summary>
    public string SmartCardDefinitionTitle
    {
      get => Entity?.SmartCardDefinition?.Title;
      set
      {
        if (SmartCardDefinitionTitle != value)
          SmartCardDefinitionId = availableSmartCardDefinitions[value];
      }
    }

    /// <summary>
    /// Title of the practice set
    /// </summary>
    public string PracticeSetTitle
    {
      get => Entity?.PracticeSet?.Title;
      set
      {
        if (PracticeSetTitle != value)
          PracticeSetId = availableDecks[value];
      }
    }

    /// <summary>
    /// The available decks
    /// </summary>
    public List<string> AvailableDecks => availableDecks.Keys.ToList();

    /// <summary>
    /// The available smart card definitions
    /// </summary>
    public List<string> AvailableSmartCardDefinitions => availableSmartCardDefinitions.Keys.ToList();

    /// <summary>
    /// The fields of the card
    /// </summary>
    public List<SmartCardField> Fields => Entity.Fields;

    /// <summary>
    /// The tags assigned to the the card
    /// </summary>
    public ObservableCollection<string> Tags { get; } = new ObservableCollection<string>();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="context">DbContext (Injected)</param>
    /// <param name="navigationManager">NavigationManager (Injected)</param>
    /// <param name="controller">Controller (Injected)</param>
    /// <param name="changeValidator">change validator (Injected)</param>
    public SmartCardEditViewModel(DbContext context, NavigationManager navigationManager, EntityControllerBase<SmartCard> controller, 
      EntityChangeValidator<SmartCard> changeValidator) 
      : base(context, navigationManager, controller, changeValidator)
    {
      Tags.CollectionChanged += (sender, e) => OnPropertyChanged(nameof(Tags));
      SmartCardDefinitionTitleProperty = new PropertyProxy(
        () => SmartCardDefinitionTitle, 
        (value) => SmartCardDefinitionTitle = value, 
        nameof(SmartCardDefinitionTitle)
      );
      PracticeSetTitleProperty = new PropertyProxy(
        () => PracticeSetTitle,
        (value) => PracticeSetTitle = value,
        nameof(PracticeSetTitle)
      );

      foreach (PracticeSet deck in context.Set<PracticeSet>())
        availableDecks.Add(deck.Title, deck.PracticeSetId);
      foreach (SmartCardDefinition smartCardDefinition in context.Set<SmartCardDefinition>())
        availableSmartCardDefinitions.Add(smartCardDefinition.Title, smartCardDefinition.SmartCardDefinitionId);
    }

    ///<inheritdoc/>
    public override void LoadOrCreateEntity(object id)
    {
      base.LoadOrCreateEntity(id);
      PracticeSetTitleProperty.Validator = (value) => ValidatePractiecSetTitle(value);
      SmartCardDefinitionTitleProperty.Validator = (value) => ValidateSmartCardDefinitionTitle(value);
    }

    ///<inheritdoc/>
    protected override void CreateNewEntity()
    {
      Entity = new SmartCard();
      SmartCardDefinitionId = SmartCardDefinition.DefaultSmartCardDefinitionId;
    }

    private void ChangeSmartCardDefinition(long id)
    {
      Entity.SmartCardDefinitionId = id;
      Entity.SmartCardDefinition = Context.Set<SmartCardDefinition>()
        .Include(definition => definition.FieldDefinitions)
        .AsNoTracking()
        .FirstOrDefault(card => card.SmartCardDefinitionId == id);
      Entity.Fields.Clear();
      foreach (SmartCardFieldDefinition fieldDefinition in Entity.SmartCardDefinition.FieldDefinitions)
        Entity.Fields.Add(new SmartCardField()
        {
          SmartCardId = Entity.SmartCardId,
          SmartCardDefinitionId = id,
          FieldName = fieldDefinition.FieldName
        });
    }

    private string ValidatePractiecSetTitle(string value) 
      => string.IsNullOrEmpty(value) ? Errors.PropertyRequired.FormatWith(PropertyNames.PracticeSet) : null;

    private string ValidateSmartCardDefinitionTitle(string value)
      => string.IsNullOrEmpty(value) ? Errors.PropertyRequired.FormatWith(PropertyNames.PracticeSet) : null;
  }
}