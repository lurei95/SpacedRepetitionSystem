using Microsoft.AspNetCore.Components;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpacedRepetitionSystem.Components.Middleware;
using SpacedRepetitionSystem.Components.ViewModels;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Components.Tests.ViewModels
{
  /// <summary>
  /// Testclass for <see cref="StatisticsViewModelBase{TEntity}"/>
  /// </summary>
  [TestClass]
  public sealed class StatisticsViewModelBaseTests
  {
    private readonly NavigationManagerMock navigationManagerMock = new NavigationManagerMock();
    private readonly Card card = new Card() { CardId = 1 };

    private sealed class TestViewModel : StatisticsViewModelBase<Card>
    {
      public override IEnumerable<PracticeHistoryEntry> DisplayedEntries => PracticeHistoryEntries;

      public TestViewModel(NavigationManager navigationManager, IApiConnector apiConnector) : base(navigationManager, apiConnector)
      { }
    }

    /// <summary>
    /// Tests that the entity is loaded on <see cref="StatisticsViewModelBase{TEntity}.InitializeAsync"/>
    /// </summary>
    [TestMethod]
    public async Task LoadsEntityOnIntializeTest()
    {
      (TestViewModel viewModel, ApiConnectorMock mock) = await InitializeViewModel();

      Assert.AreEqual(HttpMethod.Get, mock.Methods.Pop());
      Assert.AreEqual(1, (long)mock.Parameters.Pop());
      Assert.AreSame(card, viewModel.Entity);
    }

    /// <summary>
    /// Tests that the chart data is calculated correctly for <see cref="DisplayPeriod.Week"/>
    /// </summary>
    [TestMethod]
    public async Task RecalculateChartDataForWeekTest()
    {
      (TestViewModel viewModel, _) = await InitializeViewModel();

      viewModel.PracticeHistoryEntries.AddRange(new List<PracticeHistoryEntry>()
      {
        new PracticeHistoryEntry() { PracticeDate = DateTime.Today, CorrectCount = 1, HardCount = 2, WrongCount = 3 },
        new PracticeHistoryEntry() { PracticeDate = DateTime.Today.AddDays(-1), CorrectCount = 1, HardCount = 2, WrongCount = 3 },
        new PracticeHistoryEntry() { PracticeDate = DateTime.Today.AddDays(-2), CorrectCount = 1, HardCount = 2, WrongCount = 3 },
        new PracticeHistoryEntry() { PracticeDate = DateTime.Today.AddDays(-3), CorrectCount = 1, HardCount = 2, WrongCount = 3 },
        new PracticeHistoryEntry() { PracticeDate = DateTime.Today.AddDays(-4), CorrectCount = 1, HardCount = 2, WrongCount = 3 },
        new PracticeHistoryEntry() { PracticeDate = DateTime.Today.AddDays(-5), CorrectCount = 1, HardCount = 2, WrongCount = 3 },
        new PracticeHistoryEntry() { PracticeDate = DateTime.Today.AddDays(-6), CorrectCount = 1, HardCount = 2, WrongCount = 3 },
        new PracticeHistoryEntry() { PracticeDate = DateTime.Today.AddDays(-7), CorrectCount = 1, HardCount = 2, WrongCount = 3 },
      });
      viewModel.SelectedDisplayPeriodText = DisplayPeriod.Week.ToString();

      Assert.AreEqual(3, viewModel.PieChartData.Count);
      Assert.AreEqual(7, viewModel.PieChartData[0]);
      Assert.AreEqual(14, viewModel.PieChartData[1]);
      Assert.AreEqual(21, viewModel.PieChartData[2]);

      Assert.AreEqual(7, viewModel.LineChartLabels.Count);
      Assert.AreEqual(DateTime.Today.AddDays(-6).ToString("dd/MM/yyyy"), viewModel.LineChartLabels[0]);
      Assert.AreEqual(DateTime.Today.AddDays(-5).ToString("dd/MM/yyyy"), viewModel.LineChartLabels[1]);
      Assert.AreEqual(DateTime.Today.AddDays(-4).ToString("dd/MM/yyyy"), viewModel.LineChartLabels[2]);
      Assert.AreEqual(DateTime.Today.AddDays(-3).ToString("dd/MM/yyyy"), viewModel.LineChartLabels[3]);
      Assert.AreEqual(DateTime.Today.AddDays(-2).ToString("dd/MM/yyyy"), viewModel.LineChartLabels[4]);
      Assert.AreEqual(DateTime.Today.AddDays(-1).ToString("dd/MM/yyyy"), viewModel.LineChartLabels[5]);
      Assert.AreEqual(DateTime.Today.ToString("dd/MM/yyyy"), viewModel.LineChartLabels[6]);

      Assert.AreEqual(7, viewModel.LineChartData["Correct"].Count);
      Assert.AreEqual(1, viewModel.LineChartData["Correct"][0]);
      Assert.AreEqual(1, viewModel.LineChartData["Correct"][1]);
      Assert.AreEqual(1, viewModel.LineChartData["Correct"][2]);
      Assert.AreEqual(1, viewModel.LineChartData["Correct"][3]);
      Assert.AreEqual(1, viewModel.LineChartData["Correct"][4]);
      Assert.AreEqual(1, viewModel.LineChartData["Correct"][5]);
      Assert.AreEqual(1, viewModel.LineChartData["Correct"][6]);

      Assert.AreEqual(7, viewModel.LineChartData["Hard"].Count);
      Assert.AreEqual(2, viewModel.LineChartData["Hard"][0]);
      Assert.AreEqual(2, viewModel.LineChartData["Hard"][1]);
      Assert.AreEqual(2, viewModel.LineChartData["Hard"][2]);
      Assert.AreEqual(2, viewModel.LineChartData["Hard"][3]);
      Assert.AreEqual(2, viewModel.LineChartData["Hard"][4]);
      Assert.AreEqual(2, viewModel.LineChartData["Hard"][5]);
      Assert.AreEqual(2, viewModel.LineChartData["Hard"][6]);

      Assert.AreEqual(7, viewModel.LineChartData["Wrong"].Count);
      Assert.AreEqual(3, viewModel.LineChartData["Wrong"][0]);
      Assert.AreEqual(3, viewModel.LineChartData["Wrong"][1]);
      Assert.AreEqual(3, viewModel.LineChartData["Wrong"][2]);
      Assert.AreEqual(3, viewModel.LineChartData["Wrong"][3]);
      Assert.AreEqual(3, viewModel.LineChartData["Wrong"][4]);
      Assert.AreEqual(3, viewModel.LineChartData["Wrong"][5]);
      Assert.AreEqual(3, viewModel.LineChartData["Wrong"][6]);
    }

    /// <summary>
    /// Tests that the chart data is calculated correctly for <see cref="DisplayPeriod.Month"/>
    /// </summary>
    [TestMethod]
    public async Task RecalculateChartDataForMonthTest()
    {
      (TestViewModel viewModel, _) = await InitializeViewModel();

      viewModel.PracticeHistoryEntries.AddRange(new List<PracticeHistoryEntry>()
      {
        new PracticeHistoryEntry() { PracticeDate = DateTime.Today, CorrectCount = 1, HardCount = 2, WrongCount = 3 },
        new PracticeHistoryEntry() { PracticeDate = DateTime.Today.AddDays(-2), CorrectCount = 1, HardCount = 2, WrongCount = 3 },
        new PracticeHistoryEntry() { PracticeDate = DateTime.Today.AddDays(-5), CorrectCount = 1, HardCount = 2, WrongCount = 3 },
        new PracticeHistoryEntry() { PracticeDate = DateTime.Today.AddDays(-8), CorrectCount = 1, HardCount = 2, WrongCount = 3 },
        new PracticeHistoryEntry() { PracticeDate = DateTime.Today.AddDays(-12), CorrectCount = 1, HardCount = 2, WrongCount = 3 },
        new PracticeHistoryEntry() { PracticeDate = DateTime.Today.AddDays(-17), CorrectCount = 1, HardCount = 2, WrongCount = 3 },
        new PracticeHistoryEntry() { PracticeDate = DateTime.Today.AddDays(-22), CorrectCount = 1, HardCount = 2, WrongCount = 3 },
        new PracticeHistoryEntry() { PracticeDate = DateTime.Today.AddDays(-27), CorrectCount = 1, HardCount = 2, WrongCount = 3 },
        new PracticeHistoryEntry() { PracticeDate = DateTime.Today.AddDays(-42), CorrectCount = 1, HardCount = 2, WrongCount = 3 },
      });
      viewModel.SelectedDisplayPeriodText = DisplayPeriod.Week.ToString();
      viewModel.SelectedDisplayPeriodText = DisplayPeriod.Month.ToString();

      Assert.AreEqual(3, viewModel.PieChartData.Count);
      Assert.AreEqual(8, viewModel.PieChartData[0]);
      Assert.AreEqual(16, viewModel.PieChartData[1]);
      Assert.AreEqual(24, viewModel.PieChartData[2]);

      Assert.AreEqual(8, viewModel.LineChartLabels.Count);

      Assert.AreEqual(8, viewModel.LineChartData["Correct"].Count);
      Assert.AreEqual(0, viewModel.LineChartData["Correct"][0]);
      Assert.AreEqual(1, viewModel.LineChartData["Correct"][1]);
      Assert.AreEqual(1, viewModel.LineChartData["Correct"][2]);
      Assert.AreEqual(1, viewModel.LineChartData["Correct"][3]);
      Assert.AreEqual(1, viewModel.LineChartData["Correct"][4]);
      Assert.AreEqual(1, viewModel.LineChartData["Correct"][5]);
      Assert.AreEqual(1, viewModel.LineChartData["Correct"][6]);
      Assert.AreEqual(2, viewModel.LineChartData["Correct"][7]);

      Assert.AreEqual(8, viewModel.LineChartData["Hard"].Count);
      Assert.AreEqual(0, viewModel.LineChartData["Hard"][0]);
      Assert.AreEqual(2, viewModel.LineChartData["Hard"][1]);
      Assert.AreEqual(2, viewModel.LineChartData["Hard"][2]);
      Assert.AreEqual(2, viewModel.LineChartData["Hard"][3]);
      Assert.AreEqual(2, viewModel.LineChartData["Hard"][4]);
      Assert.AreEqual(2, viewModel.LineChartData["Hard"][5]);
      Assert.AreEqual(2, viewModel.LineChartData["Hard"][6]);
      Assert.AreEqual(4, viewModel.LineChartData["Hard"][7]);

      Assert.AreEqual(8, viewModel.LineChartData["Wrong"].Count);
      Assert.AreEqual(0, viewModel.LineChartData["Wrong"][0]);
      Assert.AreEqual(3, viewModel.LineChartData["Wrong"][1]);
      Assert.AreEqual(3, viewModel.LineChartData["Wrong"][2]);
      Assert.AreEqual(3, viewModel.LineChartData["Wrong"][3]);
      Assert.AreEqual(3, viewModel.LineChartData["Wrong"][4]);
      Assert.AreEqual(3, viewModel.LineChartData["Wrong"][5]);
      Assert.AreEqual(3, viewModel.LineChartData["Wrong"][6]);
      Assert.AreEqual(6, viewModel.LineChartData["Wrong"][7]);
    }

    /// <summary>
    /// Tests that the chart data is calculated correctly for <see cref="DisplayPeriod.Year"/>
    /// </summary>
    [TestMethod]
    public async Task RecalculateChartDataForYearTest()
    {
      (TestViewModel viewModel, _) = await InitializeViewModel();

      viewModel.PracticeHistoryEntries.AddRange(new List<PracticeHistoryEntry>()
      {
        new PracticeHistoryEntry() { PracticeDate = DateTime.Today, CorrectCount = 1, HardCount = 2, WrongCount = 3 },
        new PracticeHistoryEntry() { PracticeDate = DateTime.Today.AddMonths(-2), CorrectCount = 1, HardCount = 2, WrongCount = 3 },
        new PracticeHistoryEntry() { PracticeDate = DateTime.Today.AddMonths(-5), CorrectCount = 1, HardCount = 2, WrongCount = 3 },
        new PracticeHistoryEntry() { PracticeDate = DateTime.Today.AddMonths(-7), CorrectCount = 1, HardCount = 2, WrongCount = 3 },
        new PracticeHistoryEntry() { PracticeDate = DateTime.Today.AddMonths(-7).AddDays(1), CorrectCount = 1, HardCount = 2, WrongCount = 3 },
        new PracticeHistoryEntry() { PracticeDate = DateTime.Today.AddMonths(-9), CorrectCount = 1, HardCount = 2, WrongCount = 3 },
        new PracticeHistoryEntry() { PracticeDate = DateTime.Today.AddMonths(-10), CorrectCount = 1, HardCount = 2, WrongCount = 3 },
        new PracticeHistoryEntry() { PracticeDate = DateTime.Today.AddMonths(-11), CorrectCount = 1, HardCount = 2, WrongCount = 3 },
        new PracticeHistoryEntry() { PracticeDate = DateTime.Today.AddMonths(-42), CorrectCount = 1, HardCount = 2, WrongCount = 3 },
      });
      viewModel.SelectedDisplayPeriodText = DisplayPeriod.Year.ToString();

      Assert.AreEqual(3, viewModel.PieChartData.Count);
      Assert.AreEqual(8, viewModel.PieChartData[0]);
      Assert.AreEqual(16, viewModel.PieChartData[1]);
      Assert.AreEqual(24, viewModel.PieChartData[2]);

      Assert.AreEqual(12, viewModel.LineChartLabels.Count);
      for (int i = 0; i < 12; i++)
        Assert.AreEqual(DateTime.Today.AddMonths(-11 + i).ToString("MMMM"), viewModel.LineChartLabels[i]);

      Assert.AreEqual(12, viewModel.LineChartData["Correct"].Count);
      Assert.AreEqual(1, viewModel.LineChartData["Correct"][0]);
      Assert.AreEqual(1, viewModel.LineChartData["Correct"][1]);
      Assert.AreEqual(1, viewModel.LineChartData["Correct"][2]);
      Assert.AreEqual(0, viewModel.LineChartData["Correct"][3]);
      Assert.AreEqual(2, viewModel.LineChartData["Correct"][4]);
      Assert.AreEqual(0, viewModel.LineChartData["Correct"][5]);
      Assert.AreEqual(1, viewModel.LineChartData["Correct"][6]);
      Assert.AreEqual(0, viewModel.LineChartData["Correct"][7]);
      Assert.AreEqual(0, viewModel.LineChartData["Correct"][8]);
      Assert.AreEqual(1, viewModel.LineChartData["Correct"][9]);
      Assert.AreEqual(0, viewModel.LineChartData["Correct"][10]);
      Assert.AreEqual(1, viewModel.LineChartData["Correct"][11]);

      Assert.AreEqual(12, viewModel.LineChartData["Hard"].Count);
      Assert.AreEqual(2, viewModel.LineChartData["Hard"][0]);
      Assert.AreEqual(2, viewModel.LineChartData["Hard"][1]);
      Assert.AreEqual(2, viewModel.LineChartData["Hard"][2]);
      Assert.AreEqual(0, viewModel.LineChartData["Hard"][3]);
      Assert.AreEqual(4, viewModel.LineChartData["Hard"][4]);
      Assert.AreEqual(0, viewModel.LineChartData["Hard"][5]);
      Assert.AreEqual(2, viewModel.LineChartData["Hard"][6]);
      Assert.AreEqual(0, viewModel.LineChartData["Hard"][7]);
      Assert.AreEqual(0, viewModel.LineChartData["Hard"][8]);
      Assert.AreEqual(2, viewModel.LineChartData["Hard"][9]);
      Assert.AreEqual(0, viewModel.LineChartData["Hard"][10]);
      Assert.AreEqual(2, viewModel.LineChartData["Hard"][11]);

      Assert.AreEqual(12, viewModel.LineChartData["Wrong"].Count);
      Assert.AreEqual(3, viewModel.LineChartData["Wrong"][0]);
      Assert.AreEqual(3, viewModel.LineChartData["Wrong"][1]);
      Assert.AreEqual(3, viewModel.LineChartData["Wrong"][2]);
      Assert.AreEqual(0, viewModel.LineChartData["Wrong"][3]);
      Assert.AreEqual(6, viewModel.LineChartData["Wrong"][4]);
      Assert.AreEqual(0, viewModel.LineChartData["Wrong"][5]);
      Assert.AreEqual(3, viewModel.LineChartData["Wrong"][6]);
      Assert.AreEqual(0, viewModel.LineChartData["Wrong"][7]);
      Assert.AreEqual(0, viewModel.LineChartData["Wrong"][8]);
      Assert.AreEqual(3, viewModel.LineChartData["Wrong"][9]);
      Assert.AreEqual(0, viewModel.LineChartData["Wrong"][10]);
      Assert.AreEqual(3, viewModel.LineChartData["Wrong"][11]);
    }

    private async Task<(TestViewModel, ApiConnectorMock)> InitializeViewModel()
    {
      ApiConnectorMock mock = new ApiConnectorMock();
      mock.Replies.Push(new ApiReply<Card>()
      {
        WasSuccessful = true,
        Result = card
      });
      TestViewModel viewModel = new TestViewModel(navigationManagerMock, mock)
      { Id = (long)1 };
      await viewModel.InitializeAsync();
      return (viewModel, mock);
    }
  }
}
