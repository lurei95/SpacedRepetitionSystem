using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using SpacedRepetitionSystem.Entities.Entities.SmartCards;
using SpacedRepetitionSystem.Logic.Controllers.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Components.ViewModels.SmartCards
{
  /// <summary>
  /// SearchViewModel for <see cref="SmartCard"/>
  /// </summary>
  public sealed class SmartCardSearchViewModel : SearchViewModelBase<SmartCard>
  {
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="context">DbContext (Injected)</param>
    /// <param name="navigationManager">NavigationManager (Injected)</param>
    /// <param name="controller">Controller (Injected)</param>
    public SmartCardSearchViewModel(DbContext context, NavigationManager navigationManager, EntityControllerBase<SmartCard> controller) 
      : base(context, navigationManager, controller)
    { }

    ///<inheritdoc/>
    protected override async Task<List<SmartCard>> SearchCore() => await Controller.Get(null);
  }
}
