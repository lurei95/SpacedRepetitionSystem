using Microsoft.AspNetCore.Components;
using SpacedRepetitionSystem.Components.Middleware;
using SpacedRepetitionSystem.Components.ViewModels;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// ViewModel for displaying the practice statistics of a card
/// </summary>
public sealed class DeckStatisticsViewModel : StatisticsViewModelBase<Deck>
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
  public DeckStatisticsViewModel(NavigationManager navigationManager, IApiConnector apiConnector)
    : base(navigationManager, apiConnector)
  { }

  ///<inheritdoc/>
  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();
    Dictionary<string, object> parameters = new Dictionary<string, object>()
    { { nameof(Card.DeckId), Entity.DeckId } };
    List<PracticeHistoryEntry> entries = (await ApiConnector.GetAsync<PracticeHistoryEntry>(parameters)).Result;
    PracticeHistoryEntries.AddRange(entries);
    SelectableDisplayUnits.Add(nameof(Deck));
    //SelectableDisplayUnits.AddRange(Entity.C.Select(field => field.FieldName));
    SelectedDisplayUnit = SelectableDisplayUnits.First();
  }
}