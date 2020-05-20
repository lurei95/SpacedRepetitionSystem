﻿using Microsoft.AspNetCore.Components;
using SpacedRepetitionSystem.Components.Commands;
using SpacedRepetitionSystem.Components.Edits;
using SpacedRepetitionSystem.Components.Middleware;
using SpacedRepetitionSystem.Entities.Entities;
using SpacedRepetitionSystem.Entities.Validation.Core;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Components.ViewModels
{
  /// <summary>
  /// Base class for all edit view models
  /// </summary>
  /// <typeparam name="TEntity">The entity type</typeparam>
  public abstract class EditViewModelBase<TEntity> : SingleEntityViewModelBase<TEntity> where TEntity : class, IRootEntity, new()
  {
    private readonly EntityChangeValidator<TEntity> changeValidator;

    /// <summary>
    /// Whether the entity is new
    /// </summary>
    protected bool IsNewEntity { get; set; }

    /// <summary>
    /// Command for Saving the changes
    /// </summary>
    public EntitySaveCommand<TEntity> SaveChangesCommand { get; set; }

    /// <summary>
    /// Command for Saving the changes
    /// </summary>
    public EntityDeleteCommand<TEntity> DeleteCommand { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="navigationManager">NavigationManager (Injected)</param>
    /// <param name="apiConnector">ApiConnector (Injected)</param>
    /// <param name="changeValidator">Validator fpr property changes</param>
    public EditViewModelBase(NavigationManager navigationManager, IApiConnector apiConnector, 
      EntityChangeValidator<TEntity> changeValidator)
      : base(navigationManager, apiConnector)
    { this.changeValidator = changeValidator; }

    /// <summary>
    /// Loads the entity or creates a new one
    /// </summary>
    protected virtual async Task<bool> LoadOrCreateEntityAsync()
    {
      bool result;
      if (Id == null)
      {
        CreateNewEntity();
        IsNewEntity = true;
        result = true;
      }
      else
        result = await LoadEntityAsync();
      return result;
    }

    ///<inheritdoc/>
    public override async Task<bool> InitializeAsync()
    {
      bool result = await LoadOrCreateEntityAsync() && await base.InitializeAsync();
      if (!result)
        return false;

      DeleteCommand = new EntityDeleteCommand<TEntity>(ApiConnector)
      {
        CommandText = Messages.Delete,
        Entity = Entity,
        OnDeletedAction = (entity) => NavigationManager.NavigateTo("/"),
        IsEnabled = !IsNewEntity
      };
      SaveChangesCommand = new EntitySaveCommand<TEntity>(ApiConnector)
      {
        CommandText = Messages.Save,
        Entity = Entity,
        IsNewEntity = IsNewEntity
      };

      return true;
    }

    /// <summary>
    /// Creates a new Entity
    /// </summary>
    protected abstract void CreateNewEntity();

    /// <summary>
    /// Registers a PropertyProxy
    /// </summary>
    /// <param name="proxy">Proxy</param>
    protected void RegisterPropertyProperty(PropertyProxy proxy)
    { proxy.Validator = (newValue, entity) => changeValidator.Validate(proxy.PropertyName, entity, newValue); }
  }
}