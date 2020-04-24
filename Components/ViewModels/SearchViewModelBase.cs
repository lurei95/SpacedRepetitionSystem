using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using SpacedRepetitionSystem.Entities.Entities;
using SpacedRepetitionSystem.Logic.Controllers.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Components.ViewModels
{
  /// <summary>
  /// Base class for all search view models
  /// </summary>
  /// <typeparam name="TEntity">The entity type</typeparam>
  public abstract class SearchViewModelBase<TEntity> : EntityViewModelBase<TEntity>
    where TEntity : IEntity
  {
    /// <summary>
    /// Results of the search
    /// </summary>
    public List<TEntity> SearchResults { get; } = new List<TEntity>();

    /// <summary>
    /// Whether the search is currently executed
    /// </summary>
    public bool IsSearching { get; protected set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="context">DbContext (Injected)</param>
    /// <param name="navigationManager">NavigationManager (Injected)</param>
    /// <param name="apiConnector">ApiConnector (Injected)</param>
    public SearchViewModelBase(DbContext context, NavigationManager navigationManager, IApiConnector apiConnector) 
      : base(context, navigationManager, apiConnector)
    { }

    /// <summary>
    /// Executes the search
    /// </summary>
    /// <returns>async Task</returns>
    public async Task Search()
    {
      IsSearching = true;
      List<TEntity> results = await SearchCore();
      SearchResults.Clear();
      SearchResults.AddRange(results);
      IsSearching = false;
    }

    /// <summary>
    /// Performs the actual search
    /// </summary>
    /// <returns></returns>
    protected abstract Task<List<TEntity>> SearchCore();
  }
}