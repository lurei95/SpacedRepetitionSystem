using Microsoft.AspNetCore.Components;
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
    private string selectedDisplayUnit;

    /// <summary>
    /// Correct
    /// </summary>
    private static readonly string Correct = nameof(CardStatisticsViewModel.Correct);

    /// <summary>
    /// hard
    /// </summary>
    private static readonly string Hard = nameof(CardStatisticsViewModel.Hard);

    /// <summary>
    /// Wrong
    /// </summary>
    private static readonly string Wrong = nameof(CardStatisticsViewModel.Wrong);

    /// <summary>
    /// The pratice history
    /// </summary>
    public List<PracticeHistoryEntry> PracticeHistoryEntries { get; } = new List<PracticeHistoryEntry>();

    /// <summary>
    /// The displayed Entries
    /// </summary>
    public IEnumerable<PracticeHistoryEntry> DisplayedEntries
    {
      get
      {
        if (SelectedDisplayUnit == SelectableDisplayUnits[0])
          return PracticeHistoryEntries;
        return PracticeHistoryEntries.Where(entry => entry.FieldName == SelectedDisplayUnit);
      }
    }

    /// <summary>
    /// The selectable display periods
    /// </summary>
    public List<string> SelectableDisplayUnits { get; } = new List<string>() { "Card" };

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

    public DisplayPeriod SelectedDisplayPeriod { get; set; } = DisplayPeriod.Month;

    public List<string> LineChartLabels { get; } = new List<string>();

    /// <summary>
    /// Result values
    /// </summary>
    public List<int> ResultValues { get; } = new List<int>();

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
      SelectableDisplayUnits.AddRange(Entity.Fields.Select(field => field.FieldName));
      SelectedDisplayUnit = SelectableDisplayUnits.First();
    }

    /// <summary>
    /// Loads the Entity
    /// </summary>
    /// <param name="id">Id of the entity</param>
    public void LoadEntity(object id) => Entity = ApiConnector.Get<Card>(id);

    private void RecalculateChartData()
    {
      LineChartData[Correct].Clear();
      LineChartData[Wrong].Clear();
      LineChartData[Hard].Clear();
      LineChartLabels.Clear();
      ResultValues.Clear();

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

      ResultValues.Add(entries.Sum(entry => entry.CorrectCount));
      ResultValues.Add(entries.Sum(entry => entry.HardCount));
      ResultValues.Add(entries.Sum(entry => entry.WrongCount));
    }

    private void RecalculateLineChartForWeek()
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

    private void RecalculateLineChartForMonth()
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
          correct +=  DisplayedEntries.Where(entry => entry.PracticeDate.Date == date.Date.AddDays(-daysPerLabel + j + 1)).Sum(entry => entry.CorrectCount);
          hard += DisplayedEntries.Where(entry => entry.PracticeDate.Date == date.Date.AddDays(-daysPerLabel + j + 1)).Sum(entry => entry.HardCount);
          wrong += DisplayedEntries.Where(entry => entry.PracticeDate.Date == date.Date.AddDays(-daysPerLabel + j + 1)).Sum(entry => entry.WrongCount);
        }
        LineChartData[Correct].Add(correct);
        LineChartData[Hard].Add(hard);
        LineChartData[Wrong].Add(wrong);
      }
    }

    private void RecalculateLineChartForYear()
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