using Microsoft.AspNetCore.Components;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpacedRepetitionSystem.Components.Middleware;
using SpacedRepetitionSystem.Components.ViewModels;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Utility.Extensions;
using SpacedRepetitionSystem.Utility.Notification;
using SpacedRepetitionSystem.Utility.Tests.Notification;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Components.Tests.ViewModels
{
  /// <summary>
  /// Testclass for <see cref="SingleEntityViewModelBase{TEntity}"/>
  /// </summary>
  [TestClass]
  public sealed class SingleEntityViewModelBaseTests
  {
    private static NavigationManagerMock navigationManagerMock;
    private static ApiConnectorMock apiConnectorMock;
    private static NotificationProviderMock notificationProviderMock;
    private static TestViewModel viewModel;
    private static readonly Card card = new Card();

    private class TestViewModel : SingleEntityViewModelBase<Card>
    {
      public TestViewModel(NavigationManager navigationManager, IApiConnector apiConnector) : base(navigationManager, apiConnector)
      { }

      public override string Title => null;

      public async Task<bool> CallLoadAsync() => await LoadEntityAsync();
    }

    /// <summary>
    /// Method for initializing the test
    /// </summary>
    [TestInitialize]
    public void TestInitialize()
    {
      notificationProviderMock = new NotificationProviderMock();
      NotificationMessageProvider.Initialize(notificationProviderMock, 1000000);
      apiConnectorMock = new ApiConnectorMock();
      navigationManagerMock = new NavigationManagerMock();
      viewModel = new TestViewModel(navigationManagerMock, apiConnectorMock) { Id = 12 };
    }

    /// <summary>
    /// Tests <see cref="SingleEntityViewModelBase{TEntity}.LoadEntityAsync"/> when the entity is returned successfully
    /// </summary>
    [TestMethod]
    public async Task LoadEntitySuccessfullyTest()
    {
      apiConnectorMock.Replies.Push(new ApiReply<Card>() { WasSuccessful = true, Result = card });
      await LoadTestCore(true, null);
    }

    /// <summary>
    /// Tests <see cref="SingleEntityViewModelBase{TEntity}.LoadEntityAsync"/> when a generic error is returned
    /// </summary>
    [TestMethod]
    public async Task LoadEntityGenericErrorTest()
    {
      apiConnectorMock.Replies.Push(new ApiReply<Card>() { WasSuccessful = false, ResultMessage = "test" });
      await LoadTestCore(false, "test");
    }

    /// <summary>
    /// Tests <see cref="SingleEntityViewModelBase{TEntity}.LoadEntityAsync"/> when a not found error is returned
    /// </summary>
    [TestMethod]
    public async Task LoadEntityNotFoundErrorTest()
    {
      apiConnectorMock.Replies.Push(new ApiReply<Card>() { WasSuccessful = false, StatusCode = HttpStatusCode.NotFound });
      await LoadTestCore(false, Errors.EntityDoesNotExist.FormatWith("Card", 12));
    }

    private async Task LoadTestCore(bool successful, string expectedError)
    {
      bool result = await viewModel.CallLoadAsync();
      Assert.AreEqual(successful, result);
      Assert.AreEqual(12, apiConnectorMock.Parameters.Pop());
      Assert.AreEqual(apiConnectorMock.Methods, HttpMethod.Get);
      if (successful)
        Assert.AreSame(card, viewModel.Entity);
      else
      {
        Assert.IsNull(viewModel.Entity);
        Assert.AreEqual(NotificationKind.ErrorNotification, notificationProviderMock.NotificationKind);
        Assert.AreEqual(expectedError, notificationProviderMock.Message);
      }
    }
  }
}