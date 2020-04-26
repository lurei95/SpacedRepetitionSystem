using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Logic.Controllers.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Components.ViewModels.Cards
{
  /// <summary>
  /// SearchViewModel for <see cref="CardTemplate"/>
  /// </summary>
  public sealed class CardTemplateSearchViewModel : SearchViewModelBase<CardTemplate>
  {
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="context">DbContext (Injected)</param>
    /// <param name="navigationManager">NavigationManager (Injected)</param>
    /// <param name="apiConnector">ApiConnector (Injected)</param>
    public CardTemplateSearchViewModel(DbContext context, NavigationManager navigationManager, IApiConnector apiConnector)
      : base(context, navigationManager, apiConnector)
    { }

    ///<inheritdoc/>
    protected override async Task<List<CardTemplate>> SearchCore() => await ApiConnector.Get<CardTemplate>(null);
  }
}