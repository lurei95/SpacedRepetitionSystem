using Microsoft.AspNetCore.Components;
using SpacedRepetitionSystem.Components.Middleware;
using SpacedRepetitionSystem.Components.ViewModels;
using SpacedRepetitionSystem.Entities;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Utility.Notification;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.ViewModels.Statistics
{
  /// <summary>
  /// ViewModel for displaying the practice statistics of a card
  /// </summary>
  public sealed class DeckStatisticsViewModel : StatisticsViewModelBase<Deck>
  {
    private readonly Dictionary<string, long> cardIdLookup = new Dictionary<string, long>();

    /// <summary>
    /// The displayed Entries
    /// </summary>
    public override IEnumerable<PracticeHistoryEntry> DisplayedEntries
    {
      get
      {
        if (SelectedDisplayUnit == SelectableDisplayUnits[0])
          return PracticeHistoryEntries;
        return PracticeHistoryEntries.Where(entry => entry.CardId == cardIdLookup[SelectedDisplayUnit]);
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
    public override async Task<bool> InitializeAsync()
    {
      bool result = await base.InitializeAsync();
      if (!result)
        return false;

      Dictionary<string, object> parameters = new Dictionary<string, object>()
      { { nameof(Card.DeckId), Entity.DeckId } };
      ApiReply<List<PracticeHistoryEntry>> reply = await ApiConnector.GetAsync<PracticeHistoryEntry>(parameters);
      if (!reply.WasSuccessful)
      {
        NotificationMessageProvider.ShowErrorMessage(reply.ResultMessage);
        return false;
      }

      PracticeHistoryEntries.AddRange(reply.Result);
      SelectableDisplayUnits.Add(EntityNameHelper.GetName<Deck>());
      SelectableDisplayUnits.AddRange(Entity.Cards.Select(card => card.GetDisplayName()));
      foreach (Card card in Entity.Cards)
        cardIdLookup.Add(card.GetDisplayName(), card.CardId);
      SelectedDisplayUnit = SelectableDisplayUnits.First();
      return true;
    }
  }
}