using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Logic.Controllers.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Components.ViewModels.Cards
{
  /// <summary>
  /// SearchViewModel for <see cref="Deck"/>
  /// </summary>
  public sealed class DeckSearchViewModel : SearchViewModelBase<Deck>
  {
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="context">DbContext (Injected)</param>
    /// <param name="navigationManager">NavigationManager (Injected)</param>
    /// <param name="apiConnector">ApiConnector (Injected)</param>
    public DeckSearchViewModel(DbContext context, NavigationManager navigationManager, IApiConnector apiConnector) 
      : base(context, navigationManager, apiConnector)
    { }

    ///<inheritdoc/>
    protected override async Task<List<Deck>> SearchCore() => await ApiConnector.Get<Deck>(null);
  }
}
