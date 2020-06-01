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
        return PracticeHistoryEntries.Where(entry => entry.Field.FieldName == SelectedDisplayUnit);
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
    public override async Task<bool> InitializeAsync()
    {
      bool result = await base.InitializeAsync();
      if (!result)
        return false;

      Dictionary<string, object> parameters = new Dictionary<string, object>()
      { { nameof(Card.CardId), Entity.CardId } };
      ApiReply<List<PracticeHistoryEntry>> reply = await ApiConnector.GetAsync<PracticeHistoryEntry>(parameters);
      if (!reply.WasSuccessful)
      {
        NotificationMessageProvider.ShowErrorMessage(reply.ResultMessage);
        return false;
      }

      PracticeHistoryEntries.AddRange(reply.Result);
      SelectableDisplayUnits.Add(EntityNameHelper.GetName<Card>());
      SelectableDisplayUnits.AddRange(Entity.Fields.Select(field => field.FieldName));
      SelectedDisplayUnit = SelectableDisplayUnits.First();
      return true;
    }
  }
}