using Microsoft.AspNetCore.Components;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpacedRepetitionSystem.Components.Middleware;
using SpacedRepetitionSystem.Components.ViewModels;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Utility.Notification;
using SpacedRepetitionSystem.Utility.Tests.Notification;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Components.Tests.ViewModels
{
  /// <summary>
  /// Testclass for <see cref="SearchViewModelBaseTests{TEntity}"/>
  /// </summary>
  [TestClass]
  public sealed class SearchViewModelBaseTests
  {
    private readonly NavigationManagerMock navigationManagerMock = new NavigationManagerMock();
    private readonly NotificationProviderMock notificationProviderMock = new NotificationProviderMock();
    private readonly ApiConnectorMock apiConnectorMock = new ApiConnectorMock();
    private readonly List<Card> results = new List<Card>() 
    { 
      new Card() { CardId = 1 },
      new Card() { CardId = 2 }
    };

    /// <summary>
    /// Initializes the tests
    /// </summary>
    [TestInitialize]
    public void TestInitialize()
    { NotificationMessageProvider.Initialize(notificationProviderMock, 500000); }

    private sealed class TestViewModel : SearchViewModelBase<Card>
    {
      public ApiReply<List<Card>> Reply { get; set; }

      public bool SearchExecuted { get; set; }

      public TestViewModel(NavigationManager navigationManager, IApiConnector apiConnector) 
        : base(navigationManager, apiConnector)
      { }

      protected override async Task<ApiReply<List<Card>>> SearchCore()
      {
        SearchExecuted = true;
        return await Task.FromResult(Reply);
      }
    }

    /// <summary>
    /// Tests <see cref="SearchViewModelBase{TEntity}.SearchAsync"/>
    /// </summary>
    [TestMethod]
    public async Task SearchAsyncTest()
    {
      TestViewModel viewModel = new TestViewModel(navigationManagerMock, apiConnectorMock)
      { Reply = new ApiReply<List<Card>>() { WasSuccessful = true, Result = results } };
      await viewModel.SearchAsync();
      Assert.IsTrue(viewModel.SearchExecuted);
      Assert.AreEqual(2, viewModel.SearchResults.Count);
      Assert.IsFalse(viewModel.IsSearching);
      Assert.AreSame(results[0], viewModel.SearchResults[0]);
      Assert.AreSame(results[1], viewModel.SearchResults[1]);
      Assert.AreSame(results[0], viewModel.SelectedEntity);
    }

    /// <summary>
    /// Tests <see cref="SearchViewModelBase{TEntity}.SearchAsync"/> when an error is returned instead of the search results
    /// </summary>
    [TestMethod]
    public async Task SearchAsyncErrorTest()
    {
      TestViewModel viewModel = new TestViewModel(navigationManagerMock, apiConnectorMock)
      { Reply = new ApiReply<List<Card>>() { WasSuccessful = false, ResultMessage = "test-error" } };
      await viewModel.SearchAsync();
      Assert.IsTrue(viewModel.SearchExecuted);
      Assert.AreEqual(0, viewModel.SearchResults.Count);
      Assert.IsFalse(viewModel.IsSearching);
      Assert.AreEqual("test-error", notificationProviderMock.Message);
      Assert.AreEqual(NotificationKind.ErrorNotification, notificationProviderMock.NotificationKind);
    }

    /// <summary>
    /// Tests that an entity is removed from the search results if its deleted
    /// </summary>
    [TestMethod]
    public async Task EntityIsRemovedFromSearchResultsOnDeleteTest()
    {
      TestViewModel viewModel = new TestViewModel(navigationManagerMock, apiConnectorMock)
      { Reply = new ApiReply<List<Card>>() { WasSuccessful = true, Result = results } };
      await viewModel.InitializeAsync();
      viewModel.DeleteCommand.ShowDeleteDialog = false;
      apiConnectorMock.Replies.Push(new ApiReply() { WasSuccessful = true });
      viewModel.DeleteCommand.ExecuteCommand(results[0]);
      Assert.AreEqual(1, viewModel.SearchResults.Count);
      Assert.AreEqual(2, viewModel.SearchResults[0].CardId);
    }
  }
}