using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpacedRepetitionSystem.Components.Middleware;
using SpacedRepetitionSystem.Components.Tests;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.ViewModels.Cards;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Tests.ViewModels.Cards
{
  /// <summary>
  /// Testclass for <see cref="CardSearchViewModel"/>
  /// </summary>
  [TestClass]
  public sealed class CardSearchViewModelTests
  {
    /// <summary>
    /// Tests that the available decks are loaded on initialize
    /// </summary>
    [TestMethod]
    public async Task LoadsAvailableDecksOnInitializeTest()
    {
      ApiConnectorMock mock = new ApiConnectorMock();
      NavigationManagerMock navigationManagerMock = new NavigationManagerMock();
      CardSearchViewModel viewModel = new CardSearchViewModel(navigationManagerMock, mock);

      //Deck Id not already set
      mock.Replies.Push(new ApiReply<List<Card>>() 
      { 
        WasSuccessful = true,
        Result = new List<Card>(),
      });
      mock.Replies.Push(new ApiReply<List<Deck>>() 
      { 
        WasSuccessful = true, 
        Result = new List<Deck>() { new Deck() { DeckId = 1, Title = "test" } } 
      });
      await viewModel.InitializeAsync();

      Assert.AreEqual(2, viewModel.AvailableDecks.Count);
      Assert.IsTrue(viewModel.AvailableDecks.Contains("test"));
      Assert.IsTrue(viewModel.AvailableDecks.Contains(Messages.All));
      Assert.IsTrue(viewModel.DeckSelectable);
    }

    /// <summary>
    /// Tests that the available decks are loaded on initialize
    /// </summary>
    [TestMethod]
    public async Task LoadsAvailableDecksWithDeckIdSetOnInitializeTest()
    {
      ApiConnectorMock mock = new ApiConnectorMock();
      NavigationManagerMock navigationManagerMock = new NavigationManagerMock();
      CardSearchViewModel viewModel = new CardSearchViewModel(navigationManagerMock, mock)
      { DeckId = 1 };
      mock.Replies.Push(new ApiReply<List<Card>>()
      {
        WasSuccessful = true,
        Result = new List<Card>(),
      });
      mock.Replies.Push(new ApiReply<List<Deck>>()
      {
        WasSuccessful = true,
        Result = new List<Deck>() { new Deck() { DeckId = 1, Title = "test" } }
      });
      await viewModel.InitializeAsync();

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
      ApiConnectorMock mock = new ApiConnectorMock();
      NavigationManagerMock navigationManagerMock = new NavigationManagerMock();
      CardSearchViewModel viewModel = new CardSearchViewModel(navigationManagerMock, mock);
      mock.Replies.Push(new ApiReply<List<Card>>()
      {
        WasSuccessful = true,
        Result = new List<Card>() { card },
      });
      mock.Replies.Push(new ApiReply<List<Deck>>()
      {
        WasSuccessful = true,
        Result = new List<Deck>()
      });
      await viewModel.InitializeAsync();

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
      NavigationManagerMock navigationManagerMock = new NavigationManagerMock();
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
      NavigationManagerMock navigationManagerMock = new NavigationManagerMock();
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
      ApiConnectorMock mock = new ApiConnectorMock();
      NavigationManagerMock navigationManagerMock = new NavigationManagerMock();
      CardSearchViewModel viewModel = new CardSearchViewModel(navigationManagerMock, mock);
      viewModel.SelectedEntity = card;
      mock.Replies.Push(new ApiReply<List<Card>>()
      {
        WasSuccessful = true,
        Result = new List<Card>() { card },
      });
      mock.Replies.Push(new ApiReply<List<Deck>>()
      {
        WasSuccessful = true,
        Result = new List<Deck>()
      });
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
  }
}