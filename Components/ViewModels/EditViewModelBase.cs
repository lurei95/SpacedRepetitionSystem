using Microsoft.AspNetCore.Components;
using SpacedRepetitionSystem.Components.Commands;
using SpacedRepetitionSystem.Components.Edits;
using SpacedRepetitionSystem.Components.Middleware;
using SpacedRepetitionSystem.Entities.Entities;
using SpacedRepetitionSystem.Entities.Validation.Core;
using SpacedRepetitionSystem.Utility.Extensions;
using SpacedRepetitionSystem.Utility.Notification;
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
    public Command SaveChangesCommand { get; set; }

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
    {
      this.changeValidator = changeValidator;

      SaveChangesCommand = new Command() {
        CommandText = Messages.Save,
        ExecuteAction = async (param) => await SaveChanges() 
      };
    }

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

      return true;
    }

    /// <summary>
    /// Creates a new Entity
    /// </summary>
    protected abstract void CreateNewEntity();

    /// <summary>
    /// Saves the changes
    /// </summary>
    protected virtual async Task<bool> SaveChanges()
    {
      ApiReply reply;
      if (IsNewEntity)
        reply = await ApiConnector.PostAsync(Entity);
      else
        reply = await ApiConnector.PutAsync(Entity);

      if (reply.WasSuccessful)
        NotificationMessageProvider.ShowSuccessMessage(Messages.EntitySaved.FormatWith(Entity.GetDisplayName()));
      else
        NotificationMessageProvider.ShowErrorMessage(reply.ResultMessage);

      return reply.WasSuccessful;
    }

    /// <summary>
    /// Registers a PropertyProxy
    /// </summary>
    /// <param name="proxy">Proxy</param>
    protected void RegisterPropertyProperty(PropertyProxy proxy)
    { proxy.Validator = (newValue, entity) => changeValidator.Validate(proxy.PropertyName, entity, newValue); }
  }
}
