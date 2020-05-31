using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpacedRepetitionSystem.Components.Middleware;
using SpacedRepetitionSystem.Components.Tests;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Utility.Notification;
using SpacedRepetitionSystem.Utility.Tests.Notification;
using SpacedRepetitionSystem.ViewModels.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Tests.ViewModels.Statistics
{
  /// <summary>
  /// Testclass for <see cref="CardStatisticsViewModel"/>
  /// </summary>
  [TestClass]
  public sealed class CardStatisticsViewModelTests
  {
    private readonly NavigationManagerMock navigationManagerMock = new NavigationManagerMock();

    /// <summary>
    /// Tests taht <see cref="CardStatisticsViewModel.DisplayedEntries"/> are selected correctly
    /// </summary>
    [TestMethod]
    public void SelectsDisplayedEntriesCorrectlyTest()
    {
      PracticeHistoryEntry entry1 = new PracticeHistoryEntry() { Field = new CardField() { FieldName = "Field 1" } };
      PracticeHistoryEntry entry2 = new PracticeHistoryEntry() { Field = new CardField() { FieldName = "Field 2" } };
      CardStatisticsViewModel viewModel = new CardStatisticsViewModel(navigationManagerMock, new ApiConnectorMock());
      viewModel.PracticeHistoryEntries.AddRange(new List<PracticeHistoryEntry>() { entry1, entry2 });
      viewModel.SelectableDisplayUnits.Add("Card");
      viewModel.SelectableDisplayUnits.Add("Field 1");
      viewModel.SelectableDisplayUnits.Add("Field 2");

      viewModel.SelectedDisplayUnit = "Card";
      Assert.AreEqual(2, viewModel.DisplayedEntries.Count());
      Assert.IsTrue(viewModel.DisplayedEntries.Contains(entry1));
      Assert.IsTrue(viewModel.DisplayedEntries.Contains(entry2));

      viewModel.SelectedDisplayUnit = "Field 1";
      Assert.AreEqual(1, viewModel.DisplayedEntries.Count());
      Assert.IsTrue(viewModel.DisplayedEntries.Contains(entry1));

      viewModel.SelectedDisplayUnit = "Field 2";
      Assert.AreEqual(1, viewModel.DisplayedEntries.Count());
      Assert.IsTrue(viewModel.DisplayedEntries.Contains(entry2));
    }
    
    /// <summary>
    /// Tests that the practice history entries are loaded on <see cref="CardStatisticsViewModel.InitializeAsync"/>
    /// </summary>
    [TestMethod]
    public async Task LoadsPracticeHistoryEntriesOnIntializeTest()
    {
      Card card = new Card() { CardId = 1 };
      card.Fields.Add(new CardField() { FieldId = 1, FieldName = "Field 1" });
      card.Fields.Add(new CardField() { FieldId = 2, FieldName = "Field 2" });
      PracticeHistoryEntry entry1 = new PracticeHistoryEntry() { Field = new CardField() { FieldName = "Field 1" } };
      PracticeHistoryEntry entry2 = new PracticeHistoryEntry() { Field = new CardField() { FieldName = "Field 2" } };
      ApiConnectorMock mock = new ApiConnectorMock();
      mock.Replies.Push(new ApiReply<List<PracticeHistoryEntry>>()
      {
        WasSuccessful = true,
        Result = new List<PracticeHistoryEntry>() { entry1, entry2 }
      });
      mock.Replies.Push(new ApiReply<Card>()
      {
        WasSuccessful = true,
        Result = card
      });
      CardStatisticsViewModel viewModel = new CardStatisticsViewModel(navigationManagerMock, mock);
      bool result = await viewModel.InitializeAsync();

      Assert.IsTrue(result);
      Assert.AreEqual(HttpMethod.Get, mock.Methods.Pop());
      Dictionary<string, object> parameters = mock.Parameters.Pop() as Dictionary<string, object>;
      Assert.AreEqual((long)1, parameters[nameof(Card.CardId)]);
      Assert.IsTrue(viewModel.PracticeHistoryEntries.Contains(entry1));
      Assert.IsTrue(viewModel.PracticeHistoryEntries.Contains(entry2));
      Assert.AreEqual(nameof(Card), viewModel.SelectableDisplayUnits[0]);
      Assert.AreEqual(nameof(Card), viewModel.SelectedDisplayUnit);
      Assert.IsTrue(viewModel.SelectableDisplayUnits.Contains("Field 1"));
      Assert.IsTrue(viewModel.SelectableDisplayUnits.Contains("Field 2"));
    }

    /// <summary>
    /// Tests the behavior when an error is returned instead of the practice history units 
    /// </summary>
    [TestMethod]
    public async Task ErrorOnLoadingPracticeHistoryEntriesTest()
    {
      NotificationProviderMock notificationProviderMock = new NotificationProviderMock();
      NotificationMessageProvider.Initialize(notificationProviderMock, 500000);
      Card card = new Card() { CardId = 1 };
      card.Fields.Add(new CardField() { FieldId = 1, FieldName = "Field 1" });
      card.Fields.Add(new CardField() { FieldId = 2, FieldName = "Field 2" });
      ApiConnectorMock mock = new ApiConnectorMock();
      mock.Replies.Push(new ApiReply<List<PracticeHistoryEntry>>()
      {
        WasSuccessful = false,
        ResultMessage = "test-error"
      });
      mock.Replies.Push(new ApiReply<Card>()
      {
        WasSuccessful = true,
        Result = card
      });
      CardStatisticsViewModel viewModel = new CardStatisticsViewModel(navigationManagerMock, mock);
      bool result = await viewModel.InitializeAsync();

      Assert.IsFalse(result);
      Assert.AreEqual(NotificationKind.ErrorNotification, notificationProviderMock.NotificationKind);
      Assert.AreEqual("test-error", notificationProviderMock.Message);
    }
  }
}