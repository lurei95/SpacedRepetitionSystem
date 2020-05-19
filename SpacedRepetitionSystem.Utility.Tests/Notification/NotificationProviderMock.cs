using SpacedRepetitionSystem.Utility.Notification;

namespace SpacedRepetitionSystem.Utility.Tests.Notification
{
  /// <summary>
  /// Mock implementation for <see cref="INotificationProvider"/>
  /// </summary>
  public sealed class NotificationProviderMock : INotificationProvider
  {
    /// <summary>
    /// NotificationKind
    /// </summary>
    public NotificationKind? NotificationKind { get; set; }

    /// <summary>
    /// The message
    /// </summary>
    public string Message { get; set; }

    ///<inheritdoc/>
    public void NotifyMessage(NotificationKind notificationKind, string message)
    {
      NotificationKind = notificationKind;
      Message = message;
    }

    ///<inheritdoc/>
    public void ResetNotification()
    {
      NotificationKind = null;
      Message = null;
    }
  }
}