using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpacedRepetitionSystem.Components.Middleware;
using SpacedRepetitionSystem.Components.Tests;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Utility.Notification;
using SpacedRepetitionSystem.Utility.Tests.Notification;
using SpacedRepetitionSystem.ViewModels.Cards;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Tests.ViewModels.Cards
{
  /// <summary>
  /// Testclass for <see cref="CardSearchViewModel"/>
  /// </summary>
  [TestClass]
  public sealed class CardSearchViewModelTests
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
    /// Tests that the available decks are loaded on initialize
    /// </summary>
    [TestMethod]
    public async Task LoadsAvailableDecksOnInitializeTest()
    {
      ApiConnectorMock mock = CreateMockForInitialize(true, true, new List<Card>(),
        new List<Deck>() { new Deck() { DeckId = 1, Title = "test" } });
      CardSearchViewModel viewModel = new CardSearchViewModel(navigationManagerMock, mock);
      bool result = await viewModel.InitializeAsync();

      Assert.IsTrue(result);
      Assert.AreEqual(2, viewModel.AvailableDecks.Count);
      Assert.IsTrue(viewModel.AvailableDecks.Contains("test"));
      Assert.IsTrue(viewModel.AvailableDecks.Contains(Messages.All));
      Assert.IsTrue(viewModel.DeckSelectable);
    }

    /// <summary>
    /// Tests the behavior when an error is returned instead of the available decks
    /// </summary>
    [TestMethod]
    public async Task LoadsAvailableDecksErrorTest()
    {
      ApiConnectorMock mock = CreateMockForInitialize(false, true, new List<Card>(),
        new List<Deck>() { new Deck() { DeckId = 1, Title = "test" } });
      CardSearchViewModel viewModel = new CardSearchViewModel(navigationManagerMock, mock);
      bool result = await viewModel.InitializeAsync();

      Assert.IsFalse(result);
      Assert.AreEqual("test-error", notificationProviderMock.Message);
      Assert.AreEqual(NotificationKind.ErrorNotification, notificationProviderMock.NotificationKind);
    }

    /// <summary>
    /// Tests that the available decks are loaded on initialize
    /// </summary>
    [TestMethod]
    public async Task LoadsAvailableDecksWithDeckIdSetOnInitializeTest()
    {
      ApiConnectorMock mock = CreateMockForInitialize(true, true, new List<Card>(), 
        new List<Deck>() { new Deck() { DeckId = 1, Title = "test" } });
      CardSearchViewModel viewModel = new CardSearchViewModel(navigationManagerMock, mock)
      { DeckId = 1 };
      bool result = await viewModel.InitializeAsync();

      Assert.IsTrue(result);
      Assert.AreEqual(2, viewModel.AvailableDecks.Count);
      Assert.IsTrue(viewModel.AvailableDecks.Contains("test"));
      Assert.IsTrue(viewModel.AvailableDecks.Contains(Messages.All));
      Assert.IsFalse(viewModel.DeckSelectable);
      Assert.AreEqual("test", viewModel.SelectedDeckTitle);
    }

    /// <summary>
    /// Tests that the search was executed on initialize
    /// </summary>
    [TestMethod]
    public async Task DoesExecuteSearchOnInitializeTest()
    {
      Card card = new Card();
      ApiConnectorMock mock = CreateMockForInitialize(true, true, new List<Card>() { card }, new List<Deck>());
      NavigationManagerMock navigationManagerMock = new NavigationManagerMock();
      CardSearchViewModel viewModel = new CardSearchViewModel(navigationManagerMock, mock);
      bool result = await viewModel.InitializeAsync();

      Assert.IsTrue(result);
      Assert.AreEqual(1, viewModel.SearchResults.Count);
      Assert.AreSame(card, viewModel.SearchResults[0]);
    }

    /// <summary>
    /// Tests that the search parameters are send correctly when the search is executed
    /// </summary>
    [TestMethod]
    public async Task ExecuteSearchParametersTest()
    {
      ApiConnectorMock mock = new ApiConnectorMock();
      CardSearchViewModel viewModel = new CardSearchViewModel(navigationManagerMock, mock);

      mock.Replies.Push(new ApiReply<List<Card>>()
      {
        WasSuccessful = true,
        Result = new List<Card>()
      });
      await viewModel.SearchAsync();
      Dictionary<string, object> parameters = mock.Parameter as Dictionary<string, object>;
      Assert.AreEqual(0, parameters.Count);

      viewModel.SearchText = "text";
      mock.Replies.Push(new ApiReply<List<Card>>()
      {
        WasSuccessful = true,
        Result = new List<Card>()
      });
      await viewModel.SearchAsync();
      parameters = mock.Parameter as Dictionary<string, object>;
      Assert.AreEqual(1, parameters.Count);
      Assert.AreEqual("text", parameters[nameof(viewModel.SearchText)]);

      viewModel.DeckId = 1;
      mock.Replies.Push(new ApiReply<List<Card>>()
      {
        WasSuccessful = true,
        Result = new List<Card>()
      });
      await viewModel.SearchAsync();
      parameters = mock.Parameter as Dictionary<string, object>;
      Assert.AreEqual(2, parameters.Count);
      Assert.AreEqual((long)1, parameters[nameof(Deck.DeckId)]);
    }

    /// <summary>
    /// Tests the enabled logic for the new command
    /// </summary>
    [TestMethod]
    public void NewCommandEnabledTest()
    {
      ApiConnectorMock mock = new ApiConnectorMock();
      CardSearchViewModel viewModel = new CardSearchViewModel(navigationManagerMock, mock);
      Assert.IsFalse(viewModel.NewCommand.IsEnabled);
      viewModel.SelectedEntity = new Card();
      Assert.IsTrue(viewModel.NewCommand.IsEnabled);
    }

    /// <summary>
    /// Tests that all commands are initialized correctly
    /// </summary>
    [TestMethod]
    public async Task CommandsAreInitializedCorrectlyTest()
    {
      Card card = new Card() { CardId = 1, DeckId = 2 };
      ApiConnectorMock mock = CreateMockForInitialize(true, true, new List<Card>() { card }, new List<Deck>());
      NavigationManagerMock navigationManagerMock = new NavigationManagerMock();
      CardSearchViewModel viewModel = new CardSearchViewModel(navigationManagerMock, mock)
      { SelectedEntity = card };
      await viewModel.InitializeAsync();

      Assert.IsNotNull(viewModel.NewCommand.CommandText);
      Assert.IsNotNull(viewModel.NewCommand.ToolTip);
      Assert.AreEqual("/Decks/2/Cards/New", viewModel.NewCommand.TargetUriFactory.Invoke(null));

      Assert.IsNotNull(viewModel.EditCommand.CommandText);
      Assert.IsNotNull(viewModel.EditCommand.ToolTip);
      Assert.AreEqual("/Decks/2/Cards/1", viewModel.EditCommand.TargetUriFactory.Invoke(card));

      Assert.IsNotNull(viewModel.ShowStatisticsCommand.CommandText);
      Assert.IsNotNull(viewModel.ShowStatisticsCommand.ToolTip);
      Assert.AreEqual("/Decks/2/Cards/1/Statistics", viewModel.ShowStatisticsCommand.TargetUriFactory.Invoke(card));

      Assert.IsNotNull(viewModel.DeleteCommand.CommandText);
      Assert.IsNotNull(viewModel.DeleteCommand.ToolTip);
    }

    private ApiConnectorMock CreateMockForInitialize(bool getDecksSuccessful, bool searchSucessful, List<Card> cards, List<Deck> decks)
    {
      ApiConnectorMock mock = new ApiConnectorMock();
      mock.Replies.Push(new ApiReply<List<Card>>()
      {
        ResultMessage = searchSucessful ? null : "test-error",
        WasSuccessful = searchSucessful,
        Result = cards,
      });
      mock.Replies.Push(new ApiReply<List<Deck>>()
      {
        ResultMessage = getDecksSuccessful ? null : "test-error",
        WasSuccessful = getDecksSuccessful,
        Result = decks
      });
      return mock;
    }
  }
}