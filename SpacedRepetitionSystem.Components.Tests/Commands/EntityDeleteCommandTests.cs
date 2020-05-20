using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpacedRepetitionSystem.Components.Commands;
using SpacedRepetitionSystem.Components.Middleware;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Utility.Dialogs;
using SpacedRepetitionSystem.Utility.Notification;
using SpacedRepetitionSystem.Utility.Tests.Dialogs;
using SpacedRepetitionSystem.Utility.Tests.Notification;
using System.Net.Http;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Components.Tests.Commands
{
  /// <summary>
  /// Testclass for <see cref="EntityDeleteCommand{TEntity}"/>
  /// </summary>
  [TestClass]
  public sealed class EntityDeleteCommandTests
  {
    private static ApiConnectorMock mock;
    private static DialogProviderMock dialogProviderMock;
    private static EntityDeleteCommand<Card> deleteCommand;
    private static NotificationProviderMock notificationProviderMock;

    /// <summary>
    /// Method for initializing the test
    /// </summary>
    [TestInitialize]
    public void TestInitialize()
    {
      notificationProviderMock = new NotificationProviderMock();
      NotificationMessageProvider.Initialize(notificationProviderMock, 1000000);
      mock = new ApiConnectorMock();
      dialogProviderMock = new DialogProviderMock();
      deleteCommand = new EntityDeleteCommand<Card>(mock);
      ModalDialogManager.Initialize(dialogProviderMock);
    }

    /// <summary>
    /// Tests <see cref="EntityDeleteCommand{TEntity}.ExecuteCommand(object)"/>
    /// </summary>
    [TestMethod]
    public async Task ExecuteWithoutDialogSuccessfulTest()
    {
      Card card = new Card();
      deleteCommand.Entity = card;
      deleteCommand.ShowDeleteDialog = false;
      bool wasExecuted = false;
      deleteCommand.OnDeletedAction = (card1) =>
      {
        Assert.AreSame(card, card1);
        wasExecuted = true;
      };
      mock.Reply = new ApiReply() { WasSuccessful = true };
      deleteCommand.ExecuteCommand();
      await Task.Delay(200);

      Assert.AreSame(mock.Parameter, card);
      Assert.IsTrue(wasExecuted);
      Assert.IsNull(dialogProviderMock.Text);
      Assert.AreEqual(HttpMethod.Delete, mock.Method);
      Assert.AreEqual(NotificationKind.SuccessNotification, notificationProviderMock.NotificationKind);
    }

    /// <summary>
    /// Tests <see cref="EntityDeleteCommand{TEntity}.ExecuteCommand(object)"/>
    /// </summary>
    [TestMethod]
    public async Task ExecuteWithoutDialogNotSuccessfulTest()
    {
      Card card = new Card();
      deleteCommand.Entity = card;
      deleteCommand.ShowDeleteDialog = false;
      deleteCommand.OnDeletedAction = (card1) => Assert.Fail();
      mock.Reply = new ApiReply() { WasSuccessful = true, ResultMessage = "test" };
      deleteCommand.ExecuteCommand();
      await Task.Delay(200);

      Assert.AreSame(mock.Parameter, card);
      Assert.IsNull(dialogProviderMock.Text);
      Assert.AreEqual(HttpMethod.Delete, mock.Method);
      Assert.AreEqual(NotificationKind.ErrorNotification, notificationProviderMock.NotificationKind);
      Assert.AreEqual("test", notificationProviderMock.Message);
    }

    /// <summary>
    /// Tests <see cref="EntityDeleteCommand{TEntity}.ExecuteCommand(object)"/>
    /// </summary>
    [TestMethod]
    public async Task ExecuteWithoutDialogResultNoTest()
    {
      Card card = new Card();
      deleteCommand.Entity = card;
      deleteCommand.ShowDeleteDialog = true;
      deleteCommand.DeleteDialogTitle = "title";
      deleteCommand.DeleteDialogText = "text";
      deleteCommand.OnDeletedAction = (card1) => Assert.Fail();
      mock.Reply = new ApiReply() { WasSuccessful = true};
      deleteCommand.ExecuteCommand();
      dialogProviderMock.Callback.Invoke(DialogResult.No);
      await Task.Delay(200);

      Assert.IsNull(mock.Parameter);
      Assert.AreEqual("text", dialogProviderMock.Text);
      Assert.AreEqual("title", dialogProviderMock.Title);
      Assert.IsNull(notificationProviderMock.Message);
    }

    /// <summary>
    /// Tests <see cref="EntityDeleteCommand{TEntity}.ExecuteCommand(object)"/>
    /// </summary>
    [TestMethod]
    public async Task ExecuteWithoutDialogResultYesTest()
    {
      Card card = new Card();
      deleteCommand.Entity = card;
      deleteCommand.ShowDeleteDialog = true;
      deleteCommand.DeleteDialogTitle = "title";
      deleteCommand.DeleteDialogTextFactory = (card) => "text";
      bool wasExecuted = false;
      deleteCommand.OnDeletedAction = (card1) =>
      {
        wasExecuted = true;
        Assert.AreSame(card, card1);
      };
      mock.Reply = new ApiReply() { WasSuccessful = true };
      deleteCommand.ExecuteCommand();
      dialogProviderMock.Callback.Invoke(DialogResult.Yes);
      await Task.Delay(200);

      Assert.AreSame(mock.Parameter, card);
      Assert.IsTrue(wasExecuted);
      Assert.AreEqual(HttpMethod.Delete, mock.Method);
      Assert.AreEqual(NotificationKind.SuccessNotification, notificationProviderMock.NotificationKind);
      Assert.AreEqual("text", dialogProviderMock.Text);
      Assert.AreEqual("title", dialogProviderMock.Title);
    }
  }
}