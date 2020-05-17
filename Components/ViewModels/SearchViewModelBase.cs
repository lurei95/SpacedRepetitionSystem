using Microsoft.AspNetCore.Components;
using SpacedRepetitionSystem.Components.Commands;
using SpacedRepetitionSystem.Components.Middleware;
using SpacedRepetitionSystem.Entities.Entities;
using SpacedRepetitionSystem.Utility.Extensions;
using SpacedRepetitionSystem.Utility.Notification;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Components.ViewModels
{
  /// <summary>
  /// Base class for all search view models
  /// </summary>
  /// <typeparam name="TEntity">The entity type</typeparam>
  public abstract class SearchViewModelBase<TEntity> : EntityViewModelBase<TEntity>
    where TEntity : class, IRootEntity, new()
  {
    private TEntity selectedEntity;
    private bool isSearching = false;

    /// <summary>
    /// Command for deleting an entity
    /// </summary>
    public Command DeleteCommand { get; set; }

    /// <summary>
    /// Command for editing an entity
    /// </summary>
    public Command EditCommand { get; set; }

    /// <summary>
    /// Command for creating a new entity
    /// </summary>
    public Command NewCommand { get; set; }

    /// <summary>
    /// Results of the search
    /// </summary>
    public List<TEntity> SearchResults { get; } = new List<TEntity>();

    /// <summary>
    /// The currently selected entity
    /// </summary>
    public virtual TEntity SelectedEntity 
    {
      get => selectedEntity;
      set => selectedEntity = value;
    }

    /// <summary>
    /// Whether the search is currently executed
    /// </summary>
    public bool IsSearching
    {
      get => isSearching;
      set
      {
        isSearching = value;
        OnPropertyChanged();
      }
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="navigationManager">NavigationManager (Injected)</param>
    /// <param name="apiConnector">ApiConnector (Injected)</param>
    public SearchViewModelBase(NavigationManager navigationManager, IApiConnector apiConnector) 
      : base(navigationManager, apiConnector)
    {
      DeleteCommand = new Command()
      {
        CommandText = Messages.Delete,
        ExecuteAction = async (param) => await DeleteEntity(param as TEntity)
      };

      EditCommand = new Command()
      {
        CommandText = Messages.Edit,
        ExecuteAction = (param) => EditEntity(param as TEntity)
      };

      NewCommand = new Command()
      {
        CommandText = Messages.New,
        ExecuteAction = (param) => NewEntity()
      };
    }

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
      if (SearchResults.Count > 0)
        SelectedEntity = SearchResults[0];
      IsSearching = false;
    }

    /// <summary>
    /// Deletes the entity
    /// </summary>
    /// <param name="entity">The entity</param>
    protected virtual async Task DeleteEntity(TEntity entity)
    {
      if ((await ApiConnector.DeleteAsync(entity)).WasSuccessful)
      {
        NotificationMessageProvider.ShowSuccessMessage(Messages.EntityDeleted.FormatWith(entity.GetDisplayName()));
        SearchResults.Remove(entity);
        OnPropertyChanged(nameof(SearchResults));
      }
    }

    /// <summary>
    /// Opens the entity for editing
    /// </summary>
    /// <param name="entity">The entity</param>
    protected virtual void EditEntity(TEntity entity)
    { NavigationManager.NavigateTo(NavigationManager.Uri + "/" + entity.Id); }

    /// <summary>
    /// Adds a new entity
    /// </summary>
    protected virtual void NewEntity()
    { NavigationManager.NavigateTo(NavigationManager.Uri + "/New/"); }

    /// <summary>
    /// Performs the actual search
    /// </summary>
    /// <returns></returns>
    protected abstract Task<List<TEntity>> SearchCore();
  }
}