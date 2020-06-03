using Microsoft.AspNetCore.Components;
using SpacedRepetitionSystem.Components.Commands;
using SpacedRepetitionSystem.Components.Middleware;
using SpacedRepetitionSystem.Components.ViewModels;
using SpacedRepetitionSystem.Entities;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Utility.Extensions;
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

    ///<inheritdoc/>
    public override string Title => Messages.HomePageTitle;

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
    public NavigationCommand PracticeDeckCommand { get; private set; }

    /// <summary>
    /// Command for adding a new card to the deck
    /// </summary>
    public NavigationCommand AddCardCommand { get; private set; }

    /// <summary>
    /// Command for showing the practice statistics
    /// </summary>
    public NavigationCommand ShowStatisticsCommand { get; private set; }

    /// <summary>
    /// Command for creating a new deck
    /// </summary>
    public NavigationCommand NewDeckCommand { get; private set; }

    /// <summary>
    /// Command for searching decks
    /// </summary>
    public NavigationCommand SearchDecksCommand { get; private set; }

    /// <summary>
    /// Command for adding a new template
    /// </summary>
    public NavigationCommand NewTemplateCommand { get; private set; }

    /// <summary>
    /// Command for searching templates
    /// </summary>
    public NavigationCommand SearchTemplatesCommand { get; private set; }

    /// <summary>
    /// Command for searching templates
    /// </summary>
    public NavigationCommand SearchCardsCommand { get; private set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="navigationManager">NavigationManager (Injected)</param>
    /// <param name="apiConnector">ApiConnetcor (Injected)</param>
    public HomeViewModel(NavigationManager navigationManager, IApiConnector apiConnector) : base(navigationManager)
    { 
      this.apiConnector = apiConnector;

      PracticeDeckCommand = new NavigationCommand(navigationManager)
      {
        IsEnabledFunction = (parameter) => (parameter as Deck).CardCount > 0,
        TargetUriFactory = (param) => $"/Decks/{(param as Deck).DeckId}/Practice",
        CommandText = Messages.Practice,
        ToolTip = Messages.PracticeCommandToolTip.FormatWith(EntityNameHelper.GetName<Deck>())
      };
      AddCardCommand = new NavigationCommand(navigationManager)
      {
        TargetUriFactory = (param) => $"/Decks/{(long)param}/Cards/New",
        CommandText = Messages.NewCard,
        ToolTip = Components.Messages.NewCommandToolTip.FormatWith(EntityNameHelper.GetName<Card>())
      };
      ShowStatisticsCommand = new NavigationCommand(navigationManager)
      {
        TargetUriFactory = (param) => $"/Decks/{(long)param}/Statistics",
        CommandText = Messages.PracticeStatistics,
        ToolTip = Messages.ShowStatisticsCommandToolTip.FormatWith(EntityNameHelper.GetName<Deck>())
      };
      NewDeckCommand = new NavigationCommand(navigationManager)
      {
        CommandText = Messages.NewDeck,
        TargetUri = "/Decks/New",
        ToolTip = Components.Messages.NewCommandToolTip.FormatWith(EntityNameHelper.GetName<Deck>())
      };
      NewTemplateCommand = new NavigationCommand(navigationManager)
      {
        CommandText = Messages.NewTemplate,
        TargetUri = "/Templates/New",
        ToolTip = Components.Messages.NewCommandToolTip.FormatWith(EntityNameHelper.GetName<CardTemplate>())
      };
      SearchDecksCommand = new NavigationCommand(navigationManager)
      {
        CommandText = Messages.SearchDecks,
        TargetUri = "/Decks",
        ToolTip = Messages.SearchCommandToolTip.FormatWith(EntityNameHelper.GetPluralName<Deck>())
      };
      SearchTemplatesCommand = new NavigationCommand(navigationManager)
      {
        CommandText = Messages.SearchTemplates,
        TargetUri = "/Templates",
        ToolTip = Messages.SearchCommandToolTip.FormatWith(EntityNameHelper.GetPluralName<CardTemplate>())
      };
      SearchCardsCommand = new NavigationCommand(navigationManager)
      {
        CommandText = Messages.SearchCards,
        TargetUri = "/Cards",
        ToolTip = Messages.SearchCommandToolTip.FormatWith(EntityNameHelper.GetPluralName<Card>())
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
  }
}