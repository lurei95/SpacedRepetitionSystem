using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpacedRepetitionSystem.Components.Middleware;
using SpacedRepetitionSystem.Components.Tests;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Utility.Notification;
using SpacedRepetitionSystem.Utility.Tests.Notification;
using SpacedRepetitionSystem.ViewModels.Cards;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Tests.ViewModels.Cards
{
  /// <summary>
  /// Testclass for <see cref="DeckSearchViewModel"/>
  /// </summary>
  [TestClass]
  public sealed class DeckSearchViewModelTests
  {
    private readonly NavigationManagerMock navigationManagerMock = new NavigationManagerMock();
    private readonly NotificationProviderMock notificationProviderMock = new NotificationProviderMock();

    /// <summary>
    /// Initializes the tests
    /// </summary>
    [TestInitialize]
    public void TestInitialize()
    { NotificationMessageProvider.Initialize(notificationProviderMock, 500000); }

    /// <summary>
    /// Tests that the search was executed on initialize
    /// </summary>
    [TestMethod]
    public async Task DoesExecuteSearchOnInitializeTest()
    {
      Deck deck = new Deck();
      ApiConnectorMock mock = CreateMockForInitialize(new List<Deck>() { deck });
      NavigationManagerMock navigationManagerMock = new NavigationManagerMock();
      DeckSearchViewModel viewModel = new DeckSearchViewModel(navigationManagerMock, mock);
      bool result = await viewModel.InitializeAsync();

      Assert.IsTrue(result);
      Assert.AreEqual(1, viewModel.SearchResults.Count);
      Assert.AreSame(deck, viewModel.SearchResults[0]);
    }

    /// <summary>
    /// Tests that the search parameters are send correctly when the search is executed
    /// </summary>
    [TestMethod]
    public async Task ExecuteSearchParametersTest()
    {
      ApiConnectorMock mock = CreateMockForInitialize(new List<Deck>());
      DeckSearchViewModel viewModel = new DeckSearchViewModel(navigationManagerMock, mock);

      await viewModel.SearchAsync();
      Dictionary<string, object> parameters = mock.Parameters.Pop() as Dictionary<string, object>;
      Assert.AreEqual(0, parameters.Count);

      viewModel.SearchText = "text";
      mock.Replies.Push(new ApiReply<List<Deck>>()
      {
        WasSuccessful = true,
        Result = new List<Deck>()
      });
      await viewModel.SearchAsync();
      parameters = mock.Parameters.Pop() as Dictionary<string, object>;
      Assert.AreEqual(1, parameters.Count);
      Assert.AreEqual("text", parameters[nameof(viewModel.SearchText)]);
    }

    /// <summary>
    /// Tests that <see cref="DeckSearchViewModel.PracticeDeckCommand"/> is only enabled when the deck has cards
    /// </summary>
    [TestMethod]
    public async Task PracticeDeckCommandEnabledTest()
    {
      Deck deck = new Deck() { DeckId = 2 };
      ApiConnectorMock mock = CreateMockForInitialize(new List<Deck>());
      DeckSearchViewModel viewModel = new DeckSearchViewModel(navigationManagerMock, mock);
      await viewModel.InitializeAsync();

      Assert.IsFalse(viewModel.PracticeDeckCommand.IsEnabledFunction(deck));
      deck.CardCount = 1;
      Assert.IsTrue(viewModel.PracticeDeckCommand.IsEnabledFunction(deck));
    }

    /// <summary>
    /// Tests that all commands are initialized correctly
    /// </summary>
    [TestMethod]
    public async Task CommandsAreInitializedCorrectlyTest()
    {
      Deck deck = new Deck() { DeckId = 2 };
      ApiConnectorMock mock = CreateMockForInitialize(new List<Deck>());
      DeckSearchViewModel viewModel = new DeckSearchViewModel(navigationManagerMock, mock)
      { SelectedEntity = deck };
      await viewModel.InitializeAsync();

      Assert.IsNotNull(viewModel.NewCommand.CommandText);
      Assert.IsNotNull(viewModel.NewCommand.ToolTip);
      Assert.AreEqual("/New", viewModel.NewCommand.TargetUri);
      Assert.IsTrue(viewModel.NewCommand.IsRelative);

      Assert.IsNotNull(viewModel.EditCommand.CommandText);
      Assert.IsNotNull(viewModel.EditCommand.ToolTip);
      Assert.AreEqual("/2", viewModel.EditCommand.TargetUriFactory.Invoke(deck));
      Assert.IsTrue(viewModel.EditCommand.IsRelative);

      Assert.IsNotNull(viewModel.ShowStatisticsCommand.CommandText);
      Assert.IsNotNull(viewModel.ShowStatisticsCommand.ToolTip);
      Assert.AreEqual("/2/Statistics", viewModel.ShowStatisticsCommand.TargetUriFactory.Invoke(deck));
      Assert.IsTrue(viewModel.ShowStatisticsCommand.IsRelative);

      Assert.IsNotNull(viewModel.PracticeDeckCommand.CommandText);
      Assert.IsNotNull(viewModel.PracticeDeckCommand.ToolTip);
      Assert.AreEqual("/2/Practice", viewModel.PracticeDeckCommand.TargetUriFactory.Invoke(deck));
      Assert.IsTrue(viewModel.PracticeDeckCommand.IsRelative);

      Assert.IsNotNull(viewModel.DeleteCommand.CommandText);
      Assert.IsNotNull(viewModel.DeleteCommand.ToolTip);
    }

    /// <summary>
    /// Tests <see cref="DeckSearchViewModel.TogglePinned(bool, Deck)"/>
    /// </summary>
    [TestMethod]
    public void TogglePinnedTest()
    {
      Deck deck = new Deck() { DeckId = 2 };
      ApiConnectorMock mock = new ApiConnectorMock();
      DeckSearchViewModel viewModel = new DeckSearchViewModel(navigationManagerMock, mock)
      { SelectedEntity = deck };
      mock.Replies.Push(new ApiReply() { WasSuccessful = true });
      viewModel.TogglePinned(true, deck);

      Assert.IsTrue(deck.IsPinned);
      Assert.AreEqual(HttpMethod.Put, mock.Methods.Pop());
      Assert.AreSame(deck, mock.Parameters.Pop());
    }

    private ApiConnectorMock CreateMockForInitialize(List<Deck> decks)
    {
      ApiConnectorMock mock = new ApiConnectorMock();
      mock.Replies.Push(new ApiReply<List<Deck>>()
      {
        WasSuccessful = true,
        Result = decks
      });
      return mock;
    }
  }
}