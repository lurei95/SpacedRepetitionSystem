using Microsoft.AspNetCore.Components;
using SpacedRepetitionSystem.Components.Middleware;
using SpacedRepetitionSystem.Entities;
using SpacedRepetitionSystem.Entities.Entities;
using SpacedRepetitionSystem.Utility.Extensions;
using SpacedRepetitionSystem.Utility.Notification;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Components.ViewModels
{
  /// <summary>
  /// Baseclass for a ViewModel for a single entity
  /// </summary>
  /// <typeparam name="TEntity"></typeparam>
  public abstract class SingleEntityViewModelBase<TEntity> : EntityViewModelBase<TEntity> where TEntity : IRootEntity, new()
  {
    /// <summary>
    /// The Id of the entity
    /// </summary>
    public object Id { get; set; }

    /// <summary>
    /// The Entity
    /// </summary>
    public TEntity Entity { get; protected set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="navigationManager">NavigationManager (Injected)</param>
    /// <param name="apiConnector">ApiConnector (Injected)</param>
    public SingleEntityViewModelBase(NavigationManager navigationManager, IApiConnector apiConnector) : base(navigationManager, apiConnector)
    { }

    /// <summary>
    /// Loads the Entity
    /// </summary>
    protected virtual async Task<bool> LoadEntityAsync()
    {
      ApiReply<TEntity> reply = await ApiConnector.GetAsync<TEntity>(Id);
      if (reply.WasSuccessful)
      {
        Entity = reply.Result;
        return true;
      }
      else
      {
        if (reply.StatusCode == System.Net.HttpStatusCode.NotFound)
          NotificationMessageProvider.ShowErrorMessage(
            Errors.EntityDoesNotExist.FormatWith(EntityNameHelper.GetName<TEntity>(), Id));
        else
          NotificationMessageProvider.ShowErrorMessage(reply.ResultMessage);
        return false;
      }
    }

    ///<inheritdoc/>
    public override async Task<bool> InitializeAsync()
    { return await LoadEntityAsync() && await base.InitializeAsync(); }
  }
}