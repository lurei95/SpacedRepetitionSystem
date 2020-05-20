using SpacedRepetitionSystem.Components.Middleware;
using SpacedRepetitionSystem.Entities.Entities;
using SpacedRepetitionSystem.Utility.Extensions;
using SpacedRepetitionSystem.Utility.Notification;
using System;

namespace SpacedRepetitionSystem.Components.Commands
{
  /// <summary>
  /// Command for saving an entity
  /// </summary>
  /// <typeparam name="TEntity"></typeparam>
  public sealed class EntitySaveCommand<TEntity> : EntityCommandBase<TEntity> where TEntity : IRootEntity
  {
    /// <summary>
    /// Whether the entity is new
    /// </summary>
    public bool IsNewEntity { get; set; }

    /// <summary>
    /// Action to perform when the entity was successfully deleted
    /// </summary>
    public Action<TEntity> OnSavedAction { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="apiConnector">ApiConnector</param>
    public EntitySaveCommand(IApiConnector apiConnector) : base(apiConnector)
    { }

    ///<inheritdoc/>
    public override async void ExecuteCommand(object param = null)
    {
      ApiReply reply;
      if (IsNewEntity)
        reply = await ApiConnector.PostAsync(Entity);
      else
        reply = await ApiConnector.PutAsync(Entity);

      if (reply.WasSuccessful)
      {
        NotificationMessageProvider.ShowSuccessMessage(Messages.EntitySaved.FormatWith(Entity.GetDisplayName()));
        OnSavedAction.Invoke(Entity);
      }
      else
        NotificationMessageProvider.ShowErrorMessage(reply.ResultMessage);
    }
  }
}