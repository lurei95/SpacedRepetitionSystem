using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpacedRepetitionSystem.Components.Commands;
using SpacedRepetitionSystem.Components.Middleware;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Utility.Notification;
using SpacedRepetitionSystem.Utility.Tests.Notification;
using System.Net.Http;

namespace SpacedRepetitionSystem.Components.Tests.Commands
{
  /// <summary>
  /// Testclass for <see cref="EntitySaveCommand{TEntity}"/>
  /// </summary>
  [TestClass]
  public sealed class EntitySaveCommandTests
  {
    private static ApiConnectorMock mock;
    private static EntitySaveCommand<Card> saveCommand;
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
      saveCommand = new EntitySaveCommand<Card>(mock) { Entity = card };
    }

    /// <summary>
    /// Tests <see cref="EntitySaveCommand{TEntity}.ExecuteCommand(object)"/>
    /// </summary>
    [TestMethod]
    public void ExecuteWithNewEntityTest()
    { ExecuteTestCore(true, true); }

    /// <summary>
    /// Tests <see cref="EntitySaveCommand{TEntity}.ExecuteCommand(object)"/>
    /// </summary>
    [TestMethod]
    public void ExecuteWithExistingEntityTest()
    { ExecuteTestCore(false, true); }

    /// <summary>
    /// Tests <see cref="EntitySaveCommand{TEntity}.ExecuteCommand(object)"/>
    /// </summary>
    [TestMethod]
    public void ExecuteWithNewEntityErrorTest()
    { ExecuteTestCore(true, false); }

    /// <summary>
    /// Tests <see cref="EntitySaveCommand{TEntity}.ExecuteCommand(object)"/>
    /// </summary>
    [TestMethod]
    public void ExecuteWithExistingEntityErrorTest()
    { ExecuteTestCore(false, false); }

    private void ExecuteTestCore(bool newEntity, bool successful)
    {
      saveCommand.IsNewEntity = newEntity;
      bool wasExecuted = false;
      saveCommand.OnSavedAction = (card1) =>
      {
        if (successful)
        {
          Assert.AreSame(card, card1);
          wasExecuted = true;
        }
        else
          Assert.Fail();
      };

      if (successful)
        mock.Replies.Push(new ApiReply<Card>() { WasSuccessful = true, Result = card });
      else
        mock.Replies.Push(new ApiReply<Card>() { WasSuccessful = false, ResultMessage = "test", Result = card });
      saveCommand.ExecuteCommand();

      Assert.AreSame(card, mock.Parameters.Pop());
      Assert.AreEqual(newEntity ? HttpMethod.Post : HttpMethod.Put, mock.Methods.Pop());
      Assert.AreEqual(successful ? NotificationKind.SuccessNotification : NotificationKind.ErrorNotification, notificationProviderMock.NotificationKind);
      if (successful)
        Assert.IsTrue(wasExecuted);
      else
        Assert.AreEqual("test", notificationProviderMock.Message);  
    }
  }
}
