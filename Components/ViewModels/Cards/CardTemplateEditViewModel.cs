using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using SpacedRepetitionSystem.Components.Edits;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Entities.Validation.Core;
using SpacedRepetitionSystem.Logic.Controllers.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Components.ViewModels.Cards
{
  /// <summary>
  /// EditViewModel for <see cref="CardTemplate"/>
  /// </summary>
  public sealed class CardTemplateEditViewModel : EditViewModelBase<CardTemplate>
  {
    /// <summary>
    /// Property for <see cref="Entity.Title"/>
    /// </summary>
    public PropertyProxy TitleProperty { get; private set; }

    /// <summary>
    /// The fields definitions of the template
    /// </summary>
    public List<CardFieldDefinition> FieldDefinitions => Entity.FieldDefinitions;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="context">DbContext (Injected)</param>
    /// <param name="navigationManager">NavigationManager (Injected)</param>
    /// <param name="apiConnector">ApiConnector (Injected)</param>
    /// <param name="changeValidator">change validator (Injected)</param>
    public CardTemplateEditViewModel(DbContext context, NavigationManager navigationManager, IApiConnector apiConnector,
      EntityChangeValidator<CardTemplate> changeValidator)
      : base(context, navigationManager, apiConnector, changeValidator)
    { }

    ///<inheritdoc/>
    public override async Task InitializeAsync()
    {
      TitleProperty = new PropertyProxy(
        () => Entity.Title,
        (value) => Entity.Title = value,
        nameof(Entity.Title)
      );
      await base.InitializeAsync();
    }

    ///<inheritdoc/>
    protected override void CreateNewEntity()
    {
      Entity = new CardTemplate();
      Entity.FieldDefinitions.Add(new CardFieldDefinition() { FieldName = "Front" });
      Entity.FieldDefinitions.Add(new CardFieldDefinition() { FieldName = "Back" });
    }
  }
}