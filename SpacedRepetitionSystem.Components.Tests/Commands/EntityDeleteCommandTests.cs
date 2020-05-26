using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpacedRepetitionSystem.Components.Commands;
using SpacedRepetitionSystem.Components.Middleware;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Utility.Dialogs;
using SpacedRepetitionSystem.Utility.Notification;
using SpacedRepetitionSystem.Utility.Tests.Dialogs;
using SpacedRepetitionSystem.Utility.Tests.Notification;
using System.Net.Http;

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
    private static readonly Card card = new Card();

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
      deleteCommand = new EntityDeleteCommand<Card>(mock) { Entity = card };
      ModalDialogManager.Initialize(dialogProviderMock);
    }

    /// <summary>
    /// Tests <see cref="EntityDeleteCommand{TEntity}.ExecuteCommand(object)"/>
    /// </summary>
    [TestMethod]
    public void ExecuteWithoutDialogSuccessfulTest()
    { ExecuteTestCore(true, false, DialogResult.No, false); }

    /// <summary>
    /// Tests <see cref="EntityDeleteCommand{TEntity}.ExecuteCommand(object)"/>
    /// </summary>
    [TestMethod]
    public void ExecuteWithoutDialogErrorTest()
    { ExecuteTestCore(false, false, DialogResult.No, false); }

    /// <summary>
    /// Tests <see cref="EntityDeleteCommand{TEntity}.ExecuteCommand(object)"/>
    /// </summary>
    [TestMethod]
    public void ExecuteWithDialogResultNoTest()
    { ExecuteTestCore(true, true, DialogResult.No, false); }

    /// <summary>
    /// Tests <see cref="EntityDeleteCommand{TEntity}.ExecuteCommand(object)"/>
    /// </summary>
    [TestMethod]
    public void ExecuteWithDialogResultYesTest()
    { ExecuteTestCore(false, true, DialogResult.Yes, false); }

    /// <summary>
    /// Tests <see cref="EntityDeleteCommand{TEntity}.ExecuteCommand(object)"/>
    /// </summary>
    [TestMethod]
    public void ExecuteWithDialogAnTextFactoryTest()
    { ExecuteTestCore(false, true, DialogResult.Yes, true); }

    private void ExecuteTestCore(bool successful, bool useDialog, DialogResult dialogResult, bool useDialogTextFactory)
    {
      deleteCommand.ShowDeleteDialog = useDialog;
      deleteCommand.DeleteDialogTitle = "title";
      if (useDialogTextFactory)
        deleteCommand.DeleteDialogTextFactory = (card) => "text";
      else
        deleteCommand.DeleteDialogText = "text";
      bool wasExecuted = false;
      deleteCommand.OnDeletedAction = (card1) =>
      {
        if (successful && !(useDialog && dialogResult == DialogResult.No))
        {
          wasExecuted = true;
          Assert.AreSame(card, card1);
        }
        else
          Assert.Fail();
      };
      if (successful)
        mock.Replies.Push(new ApiReply() { WasSuccessful = true });
      else
        mock.Replies.Push(new ApiReply() { WasSuccessful = false, ResultMessage = "test" });
      deleteCommand.ExecuteCommand();
      if (useDialog)
        dialogProviderMock.Callback(dialogResult);
        
      if (useDialog)
      {
        Assert.AreEqual("text", dialogProviderMock.Text);
        Assert.AreEqual("title", dialogProviderMock.Title);
      }
      else
      {
        Assert.IsNull(dialogProviderMock.Text);
        Assert.IsNull(dialogProviderMock.Title);
      }

      if (!(useDialog && dialogResult == DialogResult.No))
      {
        if (successful)
        {
          Assert.AreEqual(NotificationKind.SuccessNotification, notificationProviderMock.NotificationKind);
          Assert.IsTrue(wasExecuted);
        }        
        else
        {
          Assert.AreEqual(NotificationKind.ErrorNotification, notificationProviderMock.NotificationKind);
          Assert.AreEqual("test", notificationProviderMock.Message);
        }
      }
      else if (useDialog)
        Assert.IsNull(mock.Parameter);
      else
      {
        Assert.AreSame(mock.Parameter, card);
        Assert.AreEqual(HttpMethod.Delete, mock.Method);
      }
    }
  }
}