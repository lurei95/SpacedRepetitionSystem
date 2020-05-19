using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpacedRepetitionSystem.Utility.Notification;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Utility.Tests.Notification
{
  /// <summary>
  /// Testclass for <see cref="NotificationMessageProvider"/>
  /// </summary>
  [TestClass]
  public sealed class NotificationMessageProviderTests
  {
    private NotificationProviderMock mock;

    /// <summary>
    /// Initializes the test class
    /// </summary>
    [TestInitialize]
    public void TestInitialize()
    {
      mock = new NotificationProviderMock();
      NotificationMessageProvider.Initialize(mock, 100);
    }

    /// <summary>
    /// Tests <see cref="NotificationMessageProvider.ShowSuccessMessage(string)"/>
    /// </summary>
    [TestMethod]
    public async Task ShowSuccessMessageTest()
    {
      NotificationMessageProvider.ShowSuccessMessage("test1");
      await TestCore(NotificationKind.SuccessNotification, "test1");
    }

    /// <summary>
    /// Tests <see cref="NotificationMessageProvider.ShowWarningMessage(string)"/>
    /// </summary>
    [TestMethod]
    public async Task ShowWarningMessageTest()
    {
      NotificationMessageProvider.ShowWarningMessage("test2");
      await TestCore(NotificationKind.WarningNotification, "test2");
    }

    /// <summary>
    /// Tests <see cref="NotificationMessageProvider.ShowWarningMessage(string)"/>
    /// </summary>
    [TestMethod]
    public async Task ShowInformationMessageTest()
    {
      NotificationMessageProvider.ShowInformationMessage("test3");
      await TestCore(NotificationKind.InformationNotification, "test3");
    }

    /// <summary>
    /// Tests <see cref="NotificationMessageProvider.ShowErrorMessage(string)"/>
    /// </summary>
    [TestMethod]
    public async Task ShowErrorMessageTest()
    {
      NotificationMessageProvider.ShowErrorMessage("test4");
      await TestCore(NotificationKind.ErrorNotification, "test4");
    }

    private async Task TestCore(NotificationKind notificationKind, string message)
    {
      Assert.AreEqual(notificationKind, mock.NotificationKind);
      Assert.AreEqual(message, mock.Message);
      await Task.Delay(200);
      Assert.IsNull(mock.NotificationKind);
      Assert.IsNull(mock.Message);
    }
  }
}