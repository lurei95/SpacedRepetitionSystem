using Microsoft.AspNetCore.Components;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Logic.Controllers.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Components.ViewModels.Statistics
{
  /// <summary>
  /// ViewModel for displaying the practice statistics of a card
  /// </summary>
  public sealed class CardStatisticsViewModel : StatisticsViewModelBase<Card>
  {
    /// <summary>
    /// The displayed Entries
    /// </summary>
    public override IEnumerable<PracticeHistoryEntry> DisplayedEntries
    {
      get
      {
        if (SelectedDisplayUnit == SelectableDisplayUnits[0])
          return PracticeHistoryEntries;
        return PracticeHistoryEntries.Where(entry => entry.FieldName == SelectedDisplayUnit);
      }
    }

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
      SelectableDisplayUnits.Add(nameof(Card));
      SelectableDisplayUnits.AddRange(Entity.Fields.Select(field => field.FieldName));
      SelectedDisplayUnit = SelectableDisplayUnits.First();
    }
  }
}