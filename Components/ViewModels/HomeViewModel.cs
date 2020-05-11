using Microsoft.AspNetCore.Components;
using SpacedRepetitionSystem.Components.Commands;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Logic.Controllers.Cards;
using SpacedRepetitionSystem.Logic.Controllers.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Components.ViewModels
{
  public sealed class HomeViewModel : PageViewModelBase
  {
    private readonly IApiConnector apiConnector;

    public List<Deck> PinnedDecks { get; } = new List<Deck>();

    public List<PracticeHistoryEntry> ProblemWords { get; } = new List<PracticeHistoryEntry>();

    public Dictionary<long, Deck> ProblemWordDecks { get; } = new Dictionary<long, Deck>();

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

    /// <summary>
    /// Command for creating a new deck
    /// </summary>
    public Command NewDeckCommand { get; private set; }

    /// <summary>
    /// Command for searching decks
    /// </summary>
    public Command SearchDecksCommand { get; private set; }

    /// <summary>
    /// Command for adding a new template
    /// </summary>
    public Command NewTemplateCommand { get; private set; }

    /// <summary>
    /// Command for searching templates
    /// </summary>
    public Command SearchTemplatesCommand { get; private set; }

    /// <summary>
    /// Command for searching templates
    /// </summary>
    public Command SearchCardsCommand { get; private set; }

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

      NewDeckCommand = new Command()
      {
        CommandText = Messages.NewDeck,
        ExecuteAction = (param) => NavigationManager.NavigateTo("/Decks/New/")
      };

      NewTemplateCommand = new Command()
      {
        CommandText = Messages.NewTemplate,
        ExecuteAction = (param) => NavigationManager.NavigateTo("/Templates/New/")
      };

      SearchDecksCommand = new Command()
      {
        CommandText = Messages.SearchDecks,
        ExecuteAction = (param) => NavigationManager.NavigateTo("/Decks/")
      };

      SearchTemplatesCommand = new Command()
      {
        CommandText = Messages.SearchTemplates,
        ExecuteAction = (param) => NavigationManager.NavigateTo("/Templates/")
      };

      SearchCardsCommand = new Command()
      {
        CommandText = Messages.SearchCards,
        ExecuteAction = (param) => NavigationManager.NavigateTo("/Cards/")
      };

    }

    ///<inheritdoc/>
    public override async Task InitializeAsync()
    {
      await base.InitializeAsync();
      Dictionary<string, object> parameters1 = new Dictionary<string, object>
      { { nameof(Deck.IsPinned), true } };
      PinnedDecks.AddRange(await apiConnector.Get<Deck>(parameters1));

      Dictionary<string, object> parameters2 = new Dictionary<string, object>
      { { PracticeHistoryEntriesController.ProblemWords, null } };
      ProblemWords.AddRange(await apiConnector.Get<PracticeHistoryEntry>(parameters2));
      foreach (PracticeHistoryEntry entry in ProblemWords)
        if (!ProblemWordDecks.ContainsKey(entry.DeckId))
          ProblemWordDecks.Add(entry.DeckId, apiConnector.Get<Deck>(entry.DeckId));
    }

    private void ShowStatistics(Deck deck)
    { NavigationManager.NavigateTo($"/Decks/{deck.DeckId}/Statistics/"); }

    private void AddCards(long deckId)
    { NavigationManager.NavigateTo($"/Decks/{deckId}/Cards/New"); }

    private void PracticeDeck(long deckId)
    { NavigationManager.NavigateTo($"/Decks/{deckId}/Practice"); }
  }
}
