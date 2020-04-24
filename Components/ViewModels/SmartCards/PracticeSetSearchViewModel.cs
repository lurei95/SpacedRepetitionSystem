using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using SpacedRepetitionSystem.Entities.Entities.SmartCards;
using SpacedRepetitionSystem.Logic.Controllers.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Components.ViewModels.SmartCards
{
  /// <summary>
  /// SearchViewModel for <see cref="PracticeSet"/>
  /// </summary>
  public sealed class PracticeSetSearchViewModel : SearchViewModelBase<PracticeSet>
  {
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="context">DbContext (Injected)</param>
    /// <param name="navigationManager">NavigationManager (Injected)</param>
    /// <param name="controller">Controller (Injected)</param>
    public PracticeSetSearchViewModel(DbContext context, NavigationManager navigationManager, EntityControllerBase<PracticeSet> controller) 
      : base(context, navigationManager, controller)
    { }

    ///<inheritdoc/>
    protected override async Task<List<PracticeSet>> SearchCore() => await Controller.Get(null);
  }
}
