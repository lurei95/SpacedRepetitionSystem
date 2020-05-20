using Microsoft.AspNetCore.Components;
using SpacedRepetitionSystem.Components.Commands;
using SpacedRepetitionSystem.Components.Middleware;
using SpacedRepetitionSystem.Components.ViewModels;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.ViewModels
{
  /// <summary>
  /// ViewModel for the home page
  /// </summary>
  public sealed class HomeViewModel : PageViewModelBase
  {
    private readonly IApiConnector apiConnector;

    private static readonly string ProblemWordsParameter = nameof(ProblemWords);

    /// <summary>
    /// List of pinned decks
    /// </summary>
    public List<Deck> PinnedDecks { get; } = new List<Deck>();

    /// <summary>
    /// List of problem words
    /// </summary>
    public List<PracticeHistoryEntry> ProblemWords { get; } = new List<PracticeHistoryEntry>();

    /// <summary>
    /// The decks of the problem words
    /// </summary>
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

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="navigationManager">NavigationManager (Injected)</param>
    /// <param name="apiConnector">ApiConnetcor (Injected)</param>
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
    public override async Task<bool> InitializeAsync()
    {
      await base.InitializeAsync();

      Dictionary<string, object> parameters1 = new Dictionary<string, object>
      { { nameof(Deck.IsPinned), true } };
      PinnedDecks.AddRange((await apiConnector.GetAsync<Deck>(parameters1)).Result);

      Dictionary<string, object> parameters2 = new Dictionary<string, object>{ { ProblemWordsParameter, null } };
      ProblemWords.AddRange((await apiConnector.GetAsync<PracticeHistoryEntry>(parameters2)).Result);
      foreach (PracticeHistoryEntry entry in ProblemWords)
        if (!ProblemWordDecks.ContainsKey(entry.DeckId))
          ProblemWordDecks.Add(entry.DeckId, (await apiConnector.GetAsync<Deck>(entry.DeckId)).Result);

      return true;
    }

    private void ShowStatistics(Deck deck)
    { NavigationManager.NavigateTo($"/Decks/{deck.DeckId}/Statistics/"); }

    private void AddCards(long deckId)
    { NavigationManager.NavigateTo($"/Decks/{deckId}/Cards/New"); }

    private void PracticeDeck(long deckId)
    { NavigationManager.NavigateTo($"/Decks/{deckId}/Practice"); }
  }
}