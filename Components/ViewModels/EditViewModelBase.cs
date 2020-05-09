﻿using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using SpacedRepetitionSystem.Components.Commands;
using SpacedRepetitionSystem.Components.Edits;
using SpacedRepetitionSystem.Entities.Entities;
using SpacedRepetitionSystem.Entities.Validation.Core;
using SpacedRepetitionSystem.Logic.Controllers.Core;
using SpacedRepetitionSystem.Utility.Extensions;
using SpacedRepetitionSystem.Utility.Notification;

namespace SpacedRepetitionSystem.Components.ViewModels
{
  /// <summary>
  /// Base class for all edit view models
  /// </summary>
  /// <typeparam name="TEntity">The entity type</typeparam>
  public abstract class EditViewModelBase<TEntity> : EntityViewModelBase<TEntity> where TEntity : IEntity
  {
    private readonly EntityChangeValidator<TEntity> changeValidator;

    /// <summary>
    /// Whether the entity is new
    /// </summary>
    protected bool IsNewEntity { get; set; }

    /// <summary>
    /// The Entity
    /// </summary>
    public TEntity Entity { get; protected set; }

    /// <summary>
    /// Command for Saving the changes
    /// </summary>
    public Command SaveChangesCommand { get; set; }

    /// <summary>
    /// Command for Saving the changes
    /// </summary>
    public Command DeleteCommand { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="navigationManager">NavigationManager (Injected)</param>
    /// <param name="apiConnector">ApiConnector (Injected)</param>
    /// <param name="changeValidator">Validator fpr property changes</param>
    public EditViewModelBase(NavigationManager navigationManager, IApiConnector apiConnector, 
      EntityChangeValidator<TEntity> changeValidator)
      : base(navigationManager, apiConnector)
    {
      this.changeValidator = changeValidator;

      SaveChangesCommand = new Command() {
        CommandText = Messages.Save,
        ExecuteAction = (param) => SaveChanges() 
      };

      DeleteCommand = new Command()
      {
        CommandText = Messages.Delete,
        ExecuteAction = (param) => DeleteEntity()
      };
    }

    /// <summary>
    /// Loads the entity or creates a new one
    /// </summary>
    /// <param name="id">Id of rthe entity</param>
    public virtual void LoadOrCreateEntity(object id)
    {
      if (id == null)
      {
        CreateNewEntity();
        IsNewEntity = true;
      }
      else
        LoadEntity(id);
      DeleteCommand.IsEnabled = !IsNewEntity;
    }

    /// <summary>
    /// Creates a new Entity
    /// </summary>
    protected abstract void CreateNewEntity();

    /// <summary>
    /// Loads the Entity
    /// </summary>
    /// <param name="id">Id of the entity</param>
    protected virtual void LoadEntity(object id) => Entity = ApiConnector.Get<TEntity>(id);

    /// <summary>
    /// Saves the changes
    /// </summary>
    protected virtual bool SaveChanges()
    {
      bool result;
      if (IsNewEntity)
        result = ApiConnector.Post(Entity);
      else
        result = ApiConnector.Put(Entity);
      if (result)
        NotificationMessageProvider.ShowSuccessMessage(Messages.EntitySaved.FormatWith(Entity.GetDisplayName()));
      return result;
    }

    /// <summary>
    /// Deletes the Entity
    /// </summary>
    protected virtual void DeleteEntity()
    {
      bool result = ApiConnector.Delete(Entity);
      if (result)
      {
        NotificationMessageProvider.ShowSuccessMessage(Messages.EntityDeleted.FormatWith(Entity.GetDisplayName()));
        NavigationManager.NavigateTo("/Home");
      }
    }

    /// <summary>
    /// Registers a PropertyProxy
    /// </summary>
    /// <param name="proxy">Proxy</param>
    protected void RegisterPropertyProperty(PropertyProxy proxy)
    { proxy.Validator = (newValue, entity) => changeValidator.Validate(proxy.PropertyName, entity, newValue); }
  }
}
