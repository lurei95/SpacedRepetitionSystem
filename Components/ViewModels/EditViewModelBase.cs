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
  public abstract class EditViewModelBase<TEntity> : EntityViewModelBase<TEntity> where TEntity : IRootEntity, new()
  {
    private readonly EntityChangeValidator<TEntity> changeValidator;

    /// <summary>
    /// The Id of the entity
    /// </summary>
    public object Id { get; set; }

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
        ExecuteAction = async (param) => await SaveChanges() 
      };

      DeleteCommand = new Command()
      {
        CommandText = Messages.Delete,
        ExecuteAction = async (param) => await DeleteEntity()
      };
    }

    ///<inheritdoc>/>
    public override async Task InitializeAsync()
    {
      await LoadOrCreateEntity();
      await base.InitializeAsync();
    }

    /// <summary>
    /// Loads the entity or creates a new one
    /// </summary>
    protected virtual async Task LoadOrCreateEntity()
    {
      if (Id == null)
      {
        CreateNewEntity();
        IsNewEntity = true;
      }
      else
        await LoadEntity(Id);
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
    protected virtual async Task LoadEntity(object id) => Entity = (await ApiConnector.GetAsync<TEntity>(id)).Result;

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
    /// Deletes the Entity
    /// </summary>
    protected virtual async Task DeleteEntity()
    {
      ApiReply reply = await ApiConnector.DeleteAsync(Entity);
      if (reply.WasSuccessful)
      {
        NotificationMessageProvider.ShowSuccessMessage(Messages.EntityDeleted.FormatWith(Entity.GetDisplayName()));
        NavigationManager.NavigateTo("/Home");
      }
      else
        NotificationMessageProvider.ShowErrorMessage(reply.ResultMessage);
    }

    /// <summary>
    /// Registers a PropertyProxy
    /// </summary>
    /// <param name="proxy">Proxy</param>
    protected void RegisterPropertyProperty(PropertyProxy proxy)
    { proxy.Validator = (newValue, entity) => changeValidator.Validate(proxy.PropertyName, entity, newValue); }
  }
}
