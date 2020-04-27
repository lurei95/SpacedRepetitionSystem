using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using SpacedRepetitionSystem.Components.Commands;
using SpacedRepetitionSystem.Components.Edits;
using SpacedRepetitionSystem.Entities.Entities;
using SpacedRepetitionSystem.Entities.Validation.Core;
using SpacedRepetitionSystem.Logic.Controllers.Core;
using SpacedRepetitionSystem.Utility.Extensions;
using SpacedRepetitionSystem.Utility.Notification;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

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
    /// Command for Saving the changes
    /// </summary>
    public Command CloseCommand { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="context">DBContext (Injected)</param>
    /// <param name="navigationManager">NavigationManager (Injected)</param>
    /// <param name="apiConnector">ApiConnector (Injected)</param>
    /// <param name="changeValidator">Validator fpr property changes</param>
    public EditViewModelBase(DbContext context, NavigationManager navigationManager, IApiConnector apiConnector, 
      EntityChangeValidator<TEntity> changeValidator)
      : base(context, navigationManager, apiConnector)
    {
      this.changeValidator = changeValidator;

      SaveChangesCommand = new Command() {
        CommandText = Messages.Save,
        ExecuteAction = (param) => SaveChanges() 
      };

      DeleteCommand = new Command()
      {
        Icon = "oi oi-trash",
        IsEnabled = IsNewEntity,
        ExecuteAction = (param) => DeleteEntity()
      };

      CloseCommand = new Command()
      {
        Icon = "oi oi-x",
        IsEnabled = true,
        ExecuteAction = (param) => OnClosing()
      };
    }

    public virtual async Task InitializeAsync() 
    { RegisterBindableProperties(); }

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

    protected virtual void OnClosing()
    {
      if (Context.ChangeTracker.HasChanges())
      {

      }

      NavigationManager.NavigateTo("/Home");
    }

    /// <summary>
    /// Saves the changes
    /// </summary>
    protected virtual void SaveChanges()
    {
      if (IsNewEntity)
        ApiConnector.Post(Entity);
      else
        ApiConnector.Put(Entity);
      NotificationMessageProvider.ShowSuccessMessage(Messages.EntitySaved.FormatWith(Entity.GetDisplayName()));
    }

    /// <summary>
    /// Deletes the Entity
    /// </summary>
    protected virtual void DeleteEntity()
    {
      ApiConnector.Delete(Entity);
      NotificationMessageProvider.ShowSuccessMessage(Messages.EntityDeleted.FormatWith(Entity.GetDisplayName()));
      NavigationManager.NavigateTo("/Home");
    }

    private void RegisterBindableProperties()
    {
      IEnumerable<PropertyProxy> proxies = GetType().GetTypeInfo().GetProperties()
        .Where(property => property.PropertyType == typeof(PropertyProxy))
        .Select(property => property.GetValue(this) as PropertyProxy);
      foreach (var item in proxies)
        item.Validator = (newValue) => changeValidator.Validate(item.PropertyName, Entity, newValue); 
    }
  }
}