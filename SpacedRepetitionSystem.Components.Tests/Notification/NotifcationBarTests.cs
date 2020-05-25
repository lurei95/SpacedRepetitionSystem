using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using SpacedRepetitionSystem.Components.Notification;
using SpacedRepetitionSystem.Utility.Notification;
using System.Reflection;

namespace SpacedRepetitionSystem.Components.Tests.Notification
{
  /// <summary>
  /// Testclass for <see cref="NotificationBar"/>
  /// </summary>
  [TestClass]
  public sealed class NotifcationBarTests
  {
    /// <summary>
    /// Tests that the creation of the NotificationBar initializes <see cref="NotificationMessageProvider"/>
    /// </summary>
    [TestMethod]
    public void DoesInitializeNotificationMessageProviderTest()
    {
      NotificationBar bar = new NotificationBar();
      typeof(NotificationBar).GetMethod("OnInitialized", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(bar, null);
      object value = typeof(NotificationMessageProvider)
        .GetField("notificationProvider", BindingFlags.Static | BindingFlags.NonPublic)
        .GetValue(null);
      Assert.AreSame(bar, value);
    }

    /// <summary>
    /// Tests <see cref="NotificationBar.NotifyMessage(NotificationKind, string)"/>
    /// </summary>
    [TestMethod]
    public void NotifyMessageTest()
    {
      Mock<NotificationBar> mock = new Mock<NotificationBar>();
      mock.Protected().Setup("NotifyStateChanged").Callback(() => { });
      NotificationBar bar = mock.Object;
      bar.NotifyMessage(NotificationKind.ErrorNotification, "test");
      Assert.AreEqual("test", bar.Message);
      Assert.IsTrue(bar.AlertVisible);
    }

    /// <summary>
    /// Tests <see cref="NotificationBar.ResetNotification"/>
    /// </summary>
    [TestMethod]
    public void ResetNotificationTest()
    {
      Mock<NotificationBar> mock = new Mock<NotificationBar>();
      mock.Protected().Setup("NotifyStateChanged").Callback(() => { });
      NotificationBar bar = mock.Object;
      bar.Message = "test";
      bar.AlertVisible = true;
      bar.ResetNotification();
      Assert.IsTrue(string.IsNullOrEmpty(bar.Message));
      Assert.IsFalse(bar.AlertVisible);
    }
  }
}