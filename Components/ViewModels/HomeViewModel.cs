using Microsoft.AspNetCore.Components;
using SpacedRepetitionSystem.Components.Commands;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Logic.Controllers.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Components.ViewModels
{
  public sealed class HomeViewModel : PageViewModelBase
  {
    private readonly IApiConnector apiConnector;

    public List<Deck> PinnedDecks { get; } = new List<Deck>();

    /// <summary>
    /// Command for practicing the deck
    /// </summary>
    public Command PracticeDeckCommand { get; private set; }

    /// <summary>
    /// Command for adding a new card to the deck
    /// </summary>
    public Command AddCardsCommand { get; private set; }

    /// <summary>
    /// Command for showing the practice statistics
    /// </summary>
    public Command ShowStatisticsCommand { get; private set; }

    public HomeViewModel(NavigationManager navigationManager, IApiConnector apiConnector) : base(navigationManager)
    { 
      this.apiConnector = apiConnector;

      PracticeDeckCommand = new Command()
      {
        ExecuteAction = (param) => PracticeDeck((long)param),
        CommandText = Messages.Practice,
        ToolTip = ""
      };

      AddCardsCommand = new Command()
      {
        ExecuteAction = (param) => AddCards((long)param),
        CommandText = Messages.NewCard,
        ToolTip = ""
      };

      ShowStatisticsCommand = new Command()
      {
        CommandText = Messages.PracticeStatistics,
        ExecuteAction = (param) => ShowStatistics(param as Deck)
      };
    }

    ///<inheritdoc/>
    public override async Task InitializeAsync()
    {
      await base.InitializeAsync();
      Dictionary<string, object> parameters = new Dictionary<string, object>
      { { nameof(Deck.IsPinned), true } };
      PinnedDecks.AddRange(await apiConnector.Get<Deck>(parameters));
    }

    private void ShowStatistics(Deck deck)
    { NavigationManager.NavigateTo($"/Decks/{deck.DeckId}/Statistics/"); }

    private void AddCards(long deckId)
    { NavigationManager.NavigateTo($"/Decks/{deckId}/Cards/New"); }

    private void PracticeDeck(long deckId)
    { NavigationManager.NavigateTo($"/Decks/{deckId}/Practice"); }
  }
}
