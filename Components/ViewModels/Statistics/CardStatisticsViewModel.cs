using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Logic.Controllers.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Components.ViewModels.Statistics
{
  /// <summary>
  /// ViewModel for displaying the practice statistics of a card
  /// </summary>
  public sealed class CardStatisticsViewModel : EntityViewModelBase<Card>
  {
    private static readonly string Correct = nameof(CardStatisticsViewModel.Correct);

    private static readonly string Hard = nameof(CardStatisticsViewModel.Hard);

    private static readonly string Wrong = nameof(CardStatisticsViewModel.Wrong);

    /// <summary>
    /// The pratice history
    /// </summary>
    public List<PracticeHistoryEntry> PracticeHistoryEntries { get; } = new List<PracticeHistoryEntry>();

    /// <summary>
    /// The selectable display periods
    /// </summary>
    public List<string> SelectableDisplayPeriods { get; } = new List<string>()
    {
      DisplayPeriod.Week.ToString(),
      DisplayPeriod.Month.ToString(),
      DisplayPeriod.Year.ToString()
    };

    /// <summary>
    /// Result values
    /// </summary>
    public List<int> ResultValues { get; } = new List<int>();

    public DisplayPeriod LineChartDisplayPeriod { get; set; } = DisplayPeriod.Week;

    public List<string> LineChartLabels { get; } = new List<string>();

    /// <summary>
    /// Labels for the line chart
    /// </summary>
    public Dictionary<string, List<int>> LineChartData { get; } = new Dictionary<string, List<int>>()
    {
      { Correct, new List<int>() },
      { Hard, new List<int>() },
      { Wrong, new List<int>() },
    };

    /// <summary>
    /// The Card
    /// </summary>
    public Card Entity { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="navigationManager">NavigationManager (Injected)</param>
    /// <param name="apiConnector">ApiConnetcor (Injected)</param>
    public CardStatisticsViewModel(NavigationManager navigationManager, IApiConnector apiConnector) 
      : base(navigationManager, apiConnector)
    { }

    ///<inheritdoc/>
    public override async Task InitializeAsync()
    {
      await base.InitializeAsync();
      Dictionary<string, object> parameters = new Dictionary<string, object>()
      { { nameof(Card.CardId), Entity.CardId } };
      List<PracticeHistoryEntry> entries = await ApiConnector.Get<PracticeHistoryEntry>(parameters);
      PracticeHistoryEntries.AddRange(entries);
      ResultValues.Add(PracticeHistoryEntries.Sum(entry => entry.CorrectCount));
      ResultValues.Add(PracticeHistoryEntries.Sum(entry => entry.HardCount));
      ResultValues.Add(PracticeHistoryEntries.Sum(entry => entry.WrongCount));
      RecalculateLineChartForWeek();
    }

    /// <summary>
    /// Loads the Entity
    /// </summary>
    /// <param name="id">Id of the entity</param>
    public void LoadEntity(object id) => Entity = ApiConnector.Get<Card>(id);

    private void RecalculateLineChartData()
    {
      LineChartData[Correct].Clear();
      LineChartData[Wrong].Clear();
      LineChartData[Hard].Clear();

      switch (LineChartDisplayPeriod)
      {
        case DisplayPeriod.Week:
          RecalculateLineChartForWeek();
          break;
        case DisplayPeriod.Month:
          break;
        case DisplayPeriod.Year:
          break;
        default:
          break;
      }
    }

    private void RecalculateLineChartForWeek()
    {
      for (int i = 0; i < 7; i++)
      {
        DateTime date = DateTime.Today.AddDays(-6 + i);
        LineChartLabels.Add(date.ToString("dd/MM/yyyy"));
        int correct = PracticeHistoryEntries.Where(entry => entry.PracticeDate.Date == date.Date).Sum(entry => entry.CorrectCount);
        int hard = PracticeHistoryEntries.Where(entry => entry.PracticeDate.Date == date.Date).Sum(entry => entry.HardCount);
        int wrong = PracticeHistoryEntries.Where(entry => entry.PracticeDate.Date == date.Date).Sum(entry => entry.WrongCount);
        LineChartData[Correct].Add(correct);
        LineChartData[Hard].Add(hard);
        LineChartData[Wrong].Add(wrong);
      }
    }
  }

  public enum DisplayPeriod
  {
    Week,
    Month,
    Year
  }
}