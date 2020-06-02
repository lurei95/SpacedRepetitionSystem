using Microsoft.AspNetCore.Components;
using SpacedRepetitionSystem.Components.Commands;
using SpacedRepetitionSystem.Components.Edits;
using SpacedRepetitionSystem.Components.Middleware;
using SpacedRepetitionSystem.Entities;
using SpacedRepetitionSystem.Entities.Entities;
using SpacedRepetitionSystem.Entities.Validation.Core;
using SpacedRepetitionSystem.Utility.Extensions;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Components.ViewModels
{
  /// <summary>
  /// Base class for all edit view models
  /// </summary>
  /// <typeparam name="TEntity">The entity type</typeparam>
  public abstract class EditViewModelBase<TEntity> : SingleEntityViewModelBase<TEntity> where TEntity : class, IRootEntity, new()
  {
    private bool isNewEntity;
    private readonly EntityChangeValidator<TEntity> changeValidator;

    ///<inheritdoc/>
    public override string Title => Entity?.GetDisplayName();

    /// <summary>
    /// Whether the entity is new
    /// </summary>
    public bool IsNewEntity 
    { 
      get => isNewEntity; 
      set
      {
        isNewEntity = value;
        if (SaveChangesCommand != null)
          SaveChangesCommand.IsNewEntity = value;
      }
    }

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
        ToolTip = Messages.DeleteCommandToolTip.FormatWith(EntityNameHelper.GetName<TEntity>()),
        OnDeletedAction = (entity) => NavigationManager.NavigateTo("/"),
        IsEnabled = !IsNewEntity
      };
      SaveChangesCommand = new EntitySaveCommand<TEntity>(ApiConnector)
      {
        CommandText = Messages.Save,
        Entity = Entity,
        ToolTip = Messages.SaveCommandToolTip.FormatWith(EntityNameHelper.GetName<TEntity>()),
        IsNewEntity = IsNewEntity,
        OnSavedAction = (entity) => OnEntitySaved(entity)
      };

      return true;
    }

    /// <summary>
    /// actions performened when the entity is saved
    /// </summary>
    /// <param name="entity">the returned entity</param>
    protected virtual void OnEntitySaved(TEntity entity)
    {
      Entity = entity;
      SaveChangesCommand.Entity = entity;
      IsNewEntity = false;
      OnPropertyChanged(null);
    }

    /// <summary>
    /// Creates a new Entity
    /// </summary>
    protected abstract void CreateNewEntity();

    /// <summary>
    /// Registers a PropertyProxy
    /// </summary>
    /// <param name="proxy">Proxy</param>
    protected void RegisterPropertyProxy(PropertyProxy proxy)
    { proxy.Validator = (newValue, entity) => changeValidator.Validate(proxy.PropertyName, entity, newValue); }
  }
}