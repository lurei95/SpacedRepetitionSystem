namespace SpacedRepetitionSystem.Utility.Notification
{
  /// <summary>
  /// Interface for a component that provides the possiblity to display notification messages
  /// </summary>
  public interface INotificationProvider
  {
    /// <summary>
    /// Shows the notification message
    /// </summary>
    /// <param name="notificationKind">Kind of the notification message</param>
    /// <param name="message">The notification message</param>
    void NotifyMessage(NotificationKind notificationKind, string message);

    /// <summary>
    /// Resets the notification message
    /// </summary>
    void ResetNotification();
  }
}