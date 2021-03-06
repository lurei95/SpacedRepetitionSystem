﻿using Microsoft.AspNetCore.Components;
using SpacedRepetitionSystem.Components.Commands;
using SpacedRepetitionSystem.Components.Middleware;
using SpacedRepetitionSystem.Entities;
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
    /// The search text
    /// </summary>
    public string SearchText { get; set; }

    ///<inheritdoc/>
    public override string Title => Messages.SearchPageTitle.FormatWith(EntityNameHelper.GetPluralName<TEntity>());

    /// <summary>
    /// Command for deleting an entity
    /// </summary>
    public EntityDeleteCommand<TEntity> DeleteCommand { get; set; }

    /// <summary>
    /// Command for editing an entity
    /// </summary>
    public NavigationCommand EditCommand { get; set; }

    /// <summary>
    /// Command for creating a new entity
    /// </summary>
    public NavigationCommand NewCommand { get; set; }

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
      set
      {
        if (selectedEntity != value)
          selectedEntity = value;
      }
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
      EditCommand = new NavigationCommand(navigationManager)
      {
        CommandText = Messages.Edit,
        ToolTip = Messages.EditCommandToolTip.FormatWith(EntityNameHelper.GetName<TEntity>()),
        IsRelative = true,
        TargetUriFactory = (param) => "/" + (param as TEntity).Id
      };

      NewCommand = new NavigationCommand(navigationManager)
      {
        CommandText = Messages.New,
        ToolTip = Messages.NewCommandToolTip.FormatWith(EntityNameHelper.GetName<TEntity>()),
        IsRelative = true,
        TargetUri = "/New"
      };
    }

    ///<inheritdoc/>
    public override async Task<bool> InitializeAsync()
    {
      bool result = await base.InitializeAsync();
      if (!result)
        return false;

      DeleteCommand = new EntityDeleteCommand<TEntity>(ApiConnector)
      {
        CommandText = Messages.Delete,
        ToolTip = Messages.DeleteCommandToolTip.FormatWith(EntityNameHelper.GetName<TEntity>()),
        OnDeletedAction = (entity) =>
        {
          SearchResults.Remove(entity);
          OnPropertyChanged(nameof(SearchResults));
        }
      };
      await SearchAsync();
      return true;
    }

    /// <summary>
    /// Executes the search
    /// </summary>
    /// <returns>async Task</returns>
    public async Task SearchAsync()
    {
      IsSearching = true;
      ApiReply<List<TEntity>> reply = await SearchCore();
      if (reply.WasSuccessful)
      {
        SearchResults.Clear();
        SearchResults.AddRange(reply.Result);
        if (SearchResults.Count > 0)
          SelectedEntity = SearchResults[0];
      }
      else
        NotificationMessageProvider.ShowErrorMessage(reply.ResultMessage);
      IsSearching = false;
    }

    /// <summary>
    /// Performs the actual search
    /// </summary>
    /// <returns></returns>
    protected abstract Task<ApiReply<List<TEntity>>> SearchCore();
  }
}