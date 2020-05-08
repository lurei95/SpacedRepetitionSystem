using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
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
  public sealed class CardStatisticsViewModel : EntityViewModelBase<Card>
  {
    /// <summary>
    /// The pratice history
    /// </summary>
    public List<PracticeHistoryEntry> PracticeHistoryEntries { get; } = new List<PracticeHistoryEntry>();

    /// <summary>
    /// Result values
    /// </summary>
    public List<int> ResultValues { get; } = new List<int>();

    /// <summary>
    /// The Card
    /// </summary>
    public Card Entity { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="context">DbContext (Injected)</param>
    /// <param name="navigationManager">NavigationManager (Injected)</param>
    /// <param name="apiConnector">ApiConnetcor (Injected)</param>
    public CardStatisticsViewModel(DbContext context, NavigationManager navigationManager, IApiConnector apiConnector) 
      : base(context, navigationManager, apiConnector)
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
    }

    /// <summary>
    /// Loads the Entity
    /// </summary>
    /// <param name="id">Id of the entity</param>
    public void LoadEntity(object id) => Entity = ApiConnector.Get<Card>(id);
  }
}