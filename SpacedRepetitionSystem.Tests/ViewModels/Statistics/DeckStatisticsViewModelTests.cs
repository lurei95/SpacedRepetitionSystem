using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpacedRepetitionSystem.Components.Middleware;
using SpacedRepetitionSystem.Components.Tests;
using SpacedRepetitionSystem.Entities;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Utility.Notification;
using SpacedRepetitionSystem.Utility.Tests.Notification;
using SpacedRepetitionSystem.ViewModels.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Tests.ViewModels.Statistics
{
  /// <summary>
  /// Testclass for <see cref="DeckStatisticsViewModel"/>
  /// </summary>
  [TestClass]
  public sealed class DeckStatisticsViewModelTests
  {
    private readonly NavigationManagerMock navigationManagerMock = new NavigationManagerMock();
    private readonly Deck deck = new Deck() { DeckId = 1 };
    private readonly Card card1 = new Card() { CardId = 1 };
    private readonly Card card2 = new Card() { CardId = 2 };
    private readonly PracticeHistoryEntry entry1 = new PracticeHistoryEntry() { CardId = 1 };
    private readonly PracticeHistoryEntry entry2 = new PracticeHistoryEntry() { CardId = 2 };

    /// <summary>
    /// Initializes the tests
    /// </summary>
    [TestInitialize]
    public void TestInitialize()
    {
      deck.Cards.Add(card1);
      deck.Cards.Add(card2);
    }

    /// <summary>
    /// Tests taht <see cref="DeckStatisticsViewModel.DisplayedEntries"/> are selected correctly
    /// </summary>
    [TestMethod]
    public async Task SelectsDisplayedEntriesCorrectlyTest()
    {
      ApiConnectorMock mock = CreateApiConnectorMock(deck, true, new List<PracticeHistoryEntry>() { entry1, entry2 });
      DeckStatisticsViewModel viewModel = new DeckStatisticsViewModel(navigationManagerMock, mock);
      await viewModel.InitializeAsync();

      viewModel.SelectedDisplayUnit = EntityNameHelper.GetName<Deck>();
      Assert.AreEqual(2, viewModel.DisplayedEntries.Count());
      Assert.IsTrue(viewModel.DisplayedEntries.Contains(entry1));
      Assert.IsTrue(viewModel.DisplayedEntries.Contains(entry2));

      viewModel.SelectedDisplayUnit = card1.GetDisplayName();
      Assert.AreEqual(1, viewModel.DisplayedEntries.Count());
      Assert.IsTrue(viewModel.DisplayedEntries.Contains(entry1));

      viewModel.SelectedDisplayUnit = card2.GetDisplayName();
      Assert.AreEqual(1, viewModel.DisplayedEntries.Count());
      Assert.IsTrue(viewModel.DisplayedEntries.Contains(entry2));
    }

    /// <summary>
    /// Tests that the practice history entries are loaded on <see cref="DeckStatisticsViewModel.InitializeAsync"/>
    /// </summary>
    [TestMethod]
    public async Task LoadsPracticeHistoryEntriesOnIntializeTest()
    {
      ApiConnectorMock mock = CreateApiConnectorMock(deck, true, new List<PracticeHistoryEntry>() { entry1, entry2 });
      DeckStatisticsViewModel viewModel = new DeckStatisticsViewModel(navigationManagerMock, mock);
      bool result = await viewModel.InitializeAsync();

      Assert.IsTrue(result);
      Assert.AreEqual(HttpMethod.Get, mock.Methods.Pop());
      Dictionary<string, object> parameters = mock.Parameters.Pop() as Dictionary<string, object>;
      Assert.AreEqual((long)1, parameters[nameof(Deck.DeckId)]);
      Assert.IsTrue(viewModel.PracticeHistoryEntries.Contains(entry1));
      Assert.IsTrue(viewModel.PracticeHistoryEntries.Contains(entry2));
      Assert.AreEqual(EntityNameHelper.GetName<Deck>(), viewModel.SelectableDisplayUnits[0]);
      Assert.AreEqual(EntityNameHelper.GetName<Deck>(), viewModel.SelectedDisplayUnit);
      Assert.IsTrue(viewModel.SelectableDisplayUnits.Contains(card1.GetDisplayName()));
      Assert.IsTrue(viewModel.SelectableDisplayUnits.Contains(card2.GetDisplayName()));
    }

    /// <summary>
    /// Tests the behavior when an error is returned instead of the practice history units 
    /// </summary>
    [TestMethod]
    public async Task ErrorOnLoadingPracticeHistoryEntriesTest()
    {
      NotificationProviderMock notificationProviderMock = new NotificationProviderMock();
      NotificationMessageProvider.Initialize(notificationProviderMock, 500000);
      ApiConnectorMock mock = CreateApiConnectorMock(deck, false, null);
      DeckStatisticsViewModel viewModel = new DeckStatisticsViewModel(navigationManagerMock, mock);
      bool result = await viewModel.InitializeAsync();

      Assert.IsFalse(result);
      Assert.AreEqual(NotificationKind.ErrorNotification, notificationProviderMock.NotificationKind);
      Assert.AreEqual("test-error", notificationProviderMock.Message);
    }

    private static ApiConnectorMock CreateApiConnectorMock(Deck deck, bool loadSuccessful, List<PracticeHistoryEntry> entries)
    {
      ApiConnectorMock mock = new ApiConnectorMock();
      mock.Replies.Push(new ApiReply<List<PracticeHistoryEntry>>()
      {
        WasSuccessful = loadSuccessful,
        ResultMessage = loadSuccessful ? null : "test-error",
        Result = entries
      });
      mock.Replies.Push(new ApiReply<Deck>()
      {
        WasSuccessful = true,
        Result = deck
      });
      return mock;
    }
  }
}