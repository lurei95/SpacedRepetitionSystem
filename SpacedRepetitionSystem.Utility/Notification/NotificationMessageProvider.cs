using System;
using System.Timers;

namespace SpacedRepetitionSystem.Utility.Notification
{
  /// <summary>
  /// Utility class for showing notification messages
  /// </summary>
  public static class NotificationMessageProvider
  {
    private static INotificationProvider notificationProvider;
    private static readonly Timer timer = new Timer() { Interval = 5000 };

    /// <summary>
    /// Initializes the notification message provider
    /// </summary>
    /// <param name="provider">The comnponent for showing the message</param>
    public static void Initialize(INotificationProvider provider)
    {
      if (notificationProvider != null)
        timer.Elapsed -= Timer_Elapsed;
      notificationProvider = provider ?? throw new ArgumentNullException(nameof(provider));
      timer.Elapsed += Timer_Elapsed;
    }

    /// <summary>
    /// Shows a success message
    /// </summary>
    /// <param name="message">The message</param>
    public static void ShowSuccessMessage(string message)
      => ShowNotificationCore(NotificationKind.SuccessNotification, message);

    /// <summary>
    /// Shows a information message
    /// </summary>
    /// <param name="message">The message</param>
    public static void ShowInformationMessage(string message)
      => ShowNotificationCore(NotificationKind.InformationNotification, message);

    /// <summary>
    /// Shows a warning message
    /// </summary>
    /// <param name="message">The message</param>
    public static void ShowWarningMessage(string message)
      => ShowNotificationCore(NotificationKind.WarningNotification, message);


    /// <summary>
    /// Shows a error message
    /// </summary>
    /// <param name="message">The message</param>
    public static void ShowErrorMessage(string message)
      => ShowNotificationCore(NotificationKind.ErrorNotification, message);

    private static void ShowNotificationCore(NotificationKind notificationKind, string message)
    {
      _ = notificationProvider ?? throw new ArgumentNullException(nameof(notificationProvider));
      notificationProvider.NotifyMessage(notificationKind, message);
      timer.Start();
    }

    private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
    {
      notificationProvider.ResetNotification();
      timer.Stop();
    }
  }
}