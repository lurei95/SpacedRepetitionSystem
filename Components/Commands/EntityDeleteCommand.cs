using Microsoft.AspNetCore.Components;
using SpacedRepetitionSystem.Components.Middleware;
using SpacedRepetitionSystem.Entities.Entities;
using SpacedRepetitionSystem.Utility.Dialogs;
using SpacedRepetitionSystem.Utility.Extensions;
using SpacedRepetitionSystem.Utility.Notification;
using System;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Components.Commands
{
  /// <summary>
  /// Command for deleting an entity
  /// </summary>
  /// <typeparam name="TEntity"></typeparam>
  public sealed class EntityDeleteCommand<TEntity> : EntityCommandBase<TEntity> where TEntity : class, IRootEntity
  {
    /// <summary>
    /// Whether the delete dislog should be shown
    /// </summary>
    public bool ShowDeleteDialog { get; set; } = true;

    /// <summary>
    /// Delete dialog title
    /// </summary>
    public string DeleteDialogTitle { get; set; }

    /// <summary>
    /// Delete dialog text
    /// </summary>
    public string DeleteDialogText { get; set; }

    /// <summary>
    /// Factory for creating the delete dialog text depending on the entity
    /// </summary>
    public Func<TEntity, string> DeleteDialogTextFactory { get; set; }

    /// <summary>
    /// Action to perform when the entity was successfully deleted
    /// </summary>
    public Action<TEntity> OnDeletedAction { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="apiConnector">ApiConnector</param>
    public EntityDeleteCommand(IApiConnector apiConnector) : base(apiConnector)
    { }

    ///<inheritdoc/>
    public override async void ExecuteCommand(object param = null)
    {
      TEntity entity = param is TEntity ? param as TEntity : Entity; 
      IsEnabled = false;
      if (ShowDeleteDialog)
      {
        string text = DeleteDialogTextFactory != null ? DeleteDialogTextFactory.Invoke(entity) : DeleteDialogText;
        ModalDialogManager.ShowDialog(DeleteDialogTitle, text, DialogButtons.YesNo, async (result) => 
        {
          if (result == DialogResult.Yes)
            await DeleteCoreAsync(entity);
        });
      }
      else
        await DeleteCoreAsync(entity);
    }

    private async Task DeleteCoreAsync(TEntity entity)
    {
      ApiReply reply = await ApiConnector.DeleteAsync(entity);
      if (reply.WasSuccessful)
      {
        NotificationMessageProvider.ShowSuccessMessage(Messages.EntityDeleted.FormatWith(entity.GetDisplayName()));
        OnDeletedAction?.Invoke(entity);
      }
      else
        NotificationMessageProvider.ShowErrorMessage(reply.ResultMessage);
      IsEnabled = true;
    }
  }
}