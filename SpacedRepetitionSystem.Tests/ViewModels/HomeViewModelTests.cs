using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpacedRepetitionSystem.Components.Middleware;
using SpacedRepetitionSystem.Components.Tests;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Tests.ViewModels
{
  /// <summary>
  /// Testclass for <see cref="HomeViewModel"/>
  /// </summary>
  [TestClass]
  public sealed class HomeViewModelTests
  {
    private readonly NavigationManagerMock navigationManagerMock = new NavigationManagerMock();

    /// <summary>
    /// Tests that the pinned decks are loaded on <see cref="HomeViewModel.InitializeAsync"/>
    /// </summary>
    [TestMethod]
    public async Task GetPinnedDecksOnInitializeTest()
    {
      Deck deck = new Deck();
      ApiConnectorMock mock = IntializeApiConnectorMock(new List<Deck>() { deck }, new List<PracticeHistoryEntry>(), null);
      HomeViewModel viewModel = new HomeViewModel(navigationManagerMock, mock);
      await viewModel.InitializeAsync();

      Assert.AreEqual(1, viewModel.PinnedDecks.Count);
      Assert.AreSame(deck, viewModel.PinnedDecks[0]);
      mock.Parameters.Pop();
      mock.Methods.Pop();
      Dictionary<string, object> parameters = mock.Parameters.Pop() as Dictionary<string, object>;
      Assert.IsTrue((bool)parameters[nameof(Deck.IsPinned)]);
      Assert.AreEqual(HttpMethod.Get, mock.Methods.Pop());  
    }

    /// <summary>
    /// Tests that the problem words are loaded on <see cref="HomeViewModel.InitializeAsync"/>
    /// </summary>
    [TestMethod]
    public async Task GetProblemWordsOnInitializeTest()
    {
      Deck deck = new Deck() { Title = "test", DeckId = 1 };
      PracticeHistoryEntry entry = new PracticeHistoryEntry() { DeckId = deck.DeckId };
      ApiConnectorMock mock = IntializeApiConnectorMock(new List<Deck>(), new List<PracticeHistoryEntry>() { entry }, deck);
      HomeViewModel viewModel = new HomeViewModel(navigationManagerMock, mock);
      await viewModel.InitializeAsync();

      Assert.AreEqual(1, viewModel.ProblemWords.Count);
      Assert.AreSame(entry, viewModel.ProblemWords[0]);
      Assert.AreEqual(1, viewModel.ProblemWordDecks.Count);
      Assert.AreSame(deck, viewModel.ProblemWordDecks[1]);
      Assert.AreEqual(1, (long)mock.Parameters.Pop());
      Assert.AreEqual(HttpMethod.Get, mock.Methods.Pop());
      Dictionary<string, object> parameters = mock.Parameters.Pop() as Dictionary<string, object>;
      Assert.IsTrue(parameters.ContainsKey(nameof(HomeViewModel.ProblemWords)));
      Assert.AreEqual(HttpMethod.Get, mock.Methods.Pop());
    }

    /// <summary>
    /// Tests that all commands are initialized correctly
    /// </summary>
    [TestMethod]
    public async Task CommandsAreInitializedCorrectlyTest()
    {
      ApiConnectorMock mock = IntializeApiConnectorMock(new List<Deck>(), new List<PracticeHistoryEntry>(), null);
      HomeViewModel viewModel = new HomeViewModel(navigationManagerMock, mock);
      await viewModel.InitializeAsync();

      Assert.IsNotNull(viewModel.ShowStatisticsCommand.CommandText);
      Assert.IsNotNull(viewModel.ShowStatisticsCommand.ToolTip);
      Assert.AreEqual("/Decks/1/Statistics", viewModel.ShowStatisticsCommand.TargetUriFactory((long)1));

      Assert.IsNotNull(viewModel.PracticeDeckCommand.CommandText);
      Assert.IsNotNull(viewModel.PracticeDeckCommand.ToolTip);
      Assert.AreEqual("/Decks/1/Practice", viewModel.PracticeDeckCommand.TargetUriFactory((long)1));

      Assert.IsNotNull(viewModel.AddCardCommand.CommandText);
      Assert.IsNotNull(viewModel.AddCardCommand.ToolTip);
      Assert.AreEqual("/Decks/1/Cards/New", viewModel.AddCardCommand.TargetUriFactory((long)1));

      Assert.IsNotNull(viewModel.NewDeckCommand.CommandText);
      Assert.IsNotNull(viewModel.NewDeckCommand.ToolTip);
      Assert.AreEqual("/Decks/New", viewModel.NewDeckCommand.TargetUri);

      Assert.IsNotNull(viewModel.SearchDecksCommand.CommandText);
      Assert.IsNotNull(viewModel.SearchDecksCommand.ToolTip);
      Assert.AreEqual("/Decks", viewModel.SearchDecksCommand.TargetUri);

      Assert.IsNotNull(viewModel.SearchCardsCommand.CommandText);
      Assert.IsNotNull(viewModel.SearchCardsCommand.ToolTip);
      Assert.AreEqual("/Cards", viewModel.SearchCardsCommand.TargetUri);

      Assert.IsNotNull(viewModel.NewTemplateCommand.CommandText);
      Assert.IsNotNull(viewModel.NewTemplateCommand.ToolTip);
      Assert.AreEqual("/Templates/New", viewModel.NewTemplateCommand.TargetUri);

      Assert.IsNotNull(viewModel.SearchTemplatesCommand.CommandText);
      Assert.IsNotNull(viewModel.SearchTemplatesCommand.ToolTip);
      Assert.AreEqual("/Templates", viewModel.SearchTemplatesCommand.TargetUri);
    }

    private ApiConnectorMock IntializeApiConnectorMock(List<Deck> pinnedDecks, List<PracticeHistoryEntry> problemWords, Deck deck)
    {
      ApiConnectorMock mock = new ApiConnectorMock();
      if (deck != null)
        mock.Replies.Push(new ApiReply<Deck>()
        {
          WasSuccessful = true,
          Result = deck
        });
      mock.Replies.Push(new ApiReply<List<PracticeHistoryEntry>>()
      {
        WasSuccessful = true,
        Result = problemWords
      });
      mock.Replies.Push(new ApiReply<List<Deck>>()
      {
        WasSuccessful = true,
        Result = pinnedDecks
      });
      return mock;
    }
  }
}
