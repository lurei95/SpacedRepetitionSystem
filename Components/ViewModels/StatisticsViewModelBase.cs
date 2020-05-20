using Microsoft.AspNetCore.Components;
using SpacedRepetitionSystem.Components.Middleware;
using SpacedRepetitionSystem.Entities.Entities;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Components.ViewModels
{
  /// <summary>
  /// Baseclass for a ViewModel of a statistics Page
  /// </summary>
  /// <typeparam name="TEntity"></typeparam>
  public abstract class StatisticsViewModelBase<TEntity> : EntityViewModelBase<TEntity> where TEntity : IRootEntity, new()
  {
    private string selectedDisplayUnit;

    /// <summary>
    /// The Id of the entity
    /// </summary>
    public object Id { get; set; }

    /// <summary>
    /// Correct
    /// </summary>
    private static readonly string Correct = nameof(Correct);

    /// <summary>
    /// hard
    /// </summary>
    private static readonly string Hard = nameof(Hard);

    /// <summary>
    /// Wrong
    /// </summary>
    private static readonly string Wrong = nameof(Wrong);

    /// <summary>
    /// The pratice history
    /// </summary>
    public List<PracticeHistoryEntry> PracticeHistoryEntries { get; } = new List<PracticeHistoryEntry>();

    /// <summary>
    /// The displayed Entries
    /// </summary>
    public abstract IEnumerable<PracticeHistoryEntry> DisplayedEntries { get; }

    #region Chart Settings

    /// <summary>
    /// The selectable display periods
    /// </summary>
    public List<string> SelectableDisplayUnits { get; } = new List<string>();

    /// <summary>
    /// The text of the selected display period
    /// </summary>
    public string SelectedDisplayUnit
    {
      get => selectedDisplayUnit;
      set
      {
        if (selectedDisplayUnit != value)
        {
          selectedDisplayUnit = value;
          RecalculateChartData();
        }
      }
    }

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
    /// The text of the selected display period
    /// </summary>
    public string SelectedDisplayPeriodText
    {
      get => SelectedDisplayPeriod.ToString();
      set
      {
        if (value != SelectedDisplayPeriod.ToString())
        {
          Enum.TryParse(value, out DisplayPeriod period);
          SelectedDisplayPeriod = period;
          RecalculateChartData();
        }
      }
    }

    /// <summary>
    /// The displayed time period
    /// </summary>
    public DisplayPeriod SelectedDisplayPeriod { get; set; } = DisplayPeriod.Month;

    #endregion

    #region LineChart

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
    /// Labels for the line chart
    /// </summary>
    public List<string> LineChartLabels { get; } = new List<string>();

    #endregion

    #region PieChart

    /// <summary>
    /// Result values
    /// </summary>
    public List<int> PieChartData { get; } = new List<int>();

    #endregion

    /// <summary>
    /// The entity
    /// </summary>
    public TEntity Entity { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="navigationManager">NavigationManager (Injected)</param>
    /// <param name="apiConnector">ApiConnetcor (Injected)</param>
    public StatisticsViewModelBase(NavigationManager navigationManager, IApiConnector apiConnector) : base(navigationManager, apiConnector)
    { }

    ///<inheritdoc/>
    public override async Task InitializeAsync()
    {
      await base.InitializeAsync();
      await LoadEntityAsync();
    }

    /// <summary>
    /// Loads the Entity
    /// </summary>
    public virtual async Task LoadEntityAsync() => Entity = (await ApiConnector.GetAsync<TEntity>(Id)).Result;

    /// <summary>
    /// Reacalculates the datapoint for the charts
    /// </summary>
    protected virtual void RecalculateChartData()
    {
      LineChartData[Correct].Clear();
      LineChartData[Wrong].Clear();
      LineChartData[Hard].Clear();
      LineChartLabels.Clear();
      PieChartData.Clear();

      IEnumerable<PracticeHistoryEntry> entries = DisplayedEntries; ;
      switch (SelectedDisplayPeriod)
      {
        case DisplayPeriod.Week:
          RecalculateLineChartForWeek();
          entries = entries.Where(entry => entry.PracticeDate > DateTime.Today.AddDays(-7));
          break;
        case DisplayPeriod.Month:
          RecalculateLineChartForMonth();
          entries = entries.Where(entry => entry.PracticeDate > DateTime.Today.AddMonths(-1));
          break;
        case DisplayPeriod.Year:
          RecalculateLineChartForYear();
          entries = entries.Where(entry => entry.PracticeDate > DateTime.Today.AddYears(-1));
          break;
        default:
          break;
      }

      PieChartData.Add(entries.Sum(entry => entry.CorrectCount));
      PieChartData.Add(entries.Sum(entry => entry.HardCount));
      PieChartData.Add(entries.Sum(entry => entry.WrongCount));
    }

    /// <summary>
    /// Recalculates the line chart data for <see cref="DisplayPeriod.Week"/>
    /// </summary>
    protected virtual void RecalculateLineChartForWeek()
    {
      for (int i = 0; i < 7; i++)
      {
        DateTime date = DateTime.Today.AddDays(-6 + i);
        LineChartLabels.Add(date.ToString("dd/MM/yyyy"));
        int correct = DisplayedEntries.Where(entry => entry.PracticeDate.Date == date.Date).Sum(entry => entry.CorrectCount);
        int hard = DisplayedEntries.Where(entry => entry.PracticeDate.Date == date.Date).Sum(entry => entry.HardCount);
        int wrong = DisplayedEntries.Where(entry => entry.PracticeDate.Date == date.Date).Sum(entry => entry.WrongCount);
        LineChartData[Correct].Add(correct);
        LineChartData[Hard].Add(hard);
        LineChartData[Wrong].Add(wrong);
      }
    }

    /// <summary>
    /// Recalculates the line chart data for <see cref="DisplayPeriod.Month"/>
    /// </summary>
    protected virtual void RecalculateLineChartForMonth()
    {
      var test = DateTime.Today - DateTime.Today.AddMonths(-1);
      int daysPerLabel = test.Days / 7;

      for (int i = 0; i < 8; i++)
      {
        DateTime date = DateTime.Today.AddDays(-(7 * daysPerLabel) + i * daysPerLabel);
        LineChartLabels.Add(date.ToString("dd/MM/yyyy"));
        int correct = 0;
        int hard = 0;
        int wrong = 0;
        for (int j = 0; j < daysPerLabel; j++)
        {
          correct += DisplayedEntries.Where(entry => entry.PracticeDate.Date == date.Date.AddDays(-daysPerLabel + j + 1)).Sum(entry => entry.CorrectCount);
          hard += DisplayedEntries.Where(entry => entry.PracticeDate.Date == date.Date.AddDays(-daysPerLabel + j + 1)).Sum(entry => entry.HardCount);
          wrong += DisplayedEntries.Where(entry => entry.PracticeDate.Date == date.Date.AddDays(-daysPerLabel + j + 1)).Sum(entry => entry.WrongCount);
        }
        LineChartData[Correct].Add(correct);
        LineChartData[Hard].Add(hard);
        LineChartData[Wrong].Add(wrong);
      }
    }

    /// <summary>
    /// Recalculates the line chart data for <see cref="DisplayPeriod.Year"/>
    /// </summary>
    protected virtual void RecalculateLineChartForYear()
    {
      for (int i = 0; i < 12; i++)
      {
        DateTime date = DateTime.Today.AddMonths(-11 + i);
        LineChartLabels.Add(date.ToString("MMMM"));
        int correct = DisplayedEntries.Where(entry => entry.PracticeDate.Month == date.Month).Sum(entry => entry.CorrectCount);
        int hard = DisplayedEntries.Where(entry => entry.PracticeDate.Month == date.Month).Sum(entry => entry.HardCount);
        int wrong = DisplayedEntries.Where(entry => entry.PracticeDate.Month == date.Month).Sum(entry => entry.WrongCount);
        LineChartData[Correct].Add(correct);
        LineChartData[Hard].Add(hard);
        LineChartData[Wrong].Add(wrong);
      }
    }
  }

  /// <summary>
  /// Displayey period
  /// </summary>
  public enum DisplayPeriod
  {
    /// <summary>
    /// 1 week
    /// </summary>
    Week,
    /// <summary>
    /// 1 month
    /// </summary>
    Month,
    /// <summary>
    /// 1 year
    /// </summary>
    Year
  }
}