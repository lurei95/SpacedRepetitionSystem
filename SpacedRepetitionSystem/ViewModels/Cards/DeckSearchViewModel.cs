using Microsoft.AspNetCore.Components;
using SpacedRepetitionSystem.Components.Commands;
using SpacedRepetitionSystem.Components.Middleware;
using SpacedRepetitionSystem.Components.ViewModels;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Utility.Dialogs;
using SpacedRepetitionSystem.Utility.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.ViewModels.Cards
{
  /// <summary>
  /// SearchViewModel for <see cref="Deck"/>
  /// </summary>
  public sealed class DeckSearchViewModel : SearchViewModelBase<Deck>
  {
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
    /// Constructor
    /// </summary>
    /// <param name="navigationManager">NavigationManager (Injected)</param>
    /// <param name="apiConnector">ApiConnector (Injected)</param>
    public DeckSearchViewModel(NavigationManager navigationManager, IApiConnector apiConnector) 
      : base(navigationManager, apiConnector)
    {
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

    /// <summary>
    /// Toggles <see cref="Deck.IsPinned"/> for the deck
    /// </summary>
    /// <param name="value">the new value</param>
    /// <param name="deck">the deck</param>
    public async void TogglePinned(bool value, Deck deck)
    {
      deck.IsPinned = value;
      await ApiConnector.PutAsync(deck);
    }

    ///<inheritdoc/>
    protected override async Task DeleteEntity(Deck entity)
    {
      ModalDialogManager.ShowDialog(Messages.DeleteDeckDialogTitle, 
        Messages.DeleteDeckDialogText.FormatWith(entity.Title), DialogButtons.YesNo, async (result) =>
      {
        if (result == DialogResult.Yes)
          await base.DeleteEntity(entity);
      });
      await Task.FromResult<object>(null);
    }

    ///<inheritdoc/>
    protected override async Task<List<Deck>> SearchCore() 
      => (await ApiConnector.GetAsync<Deck>(new Dictionary<string, object>())).Result;

    private void ShowStatistics(Deck deck)
    { NavigationManager.NavigateTo("/Decks/" + deck.DeckId + "/Statistics/"); }

    private void AddCards(long deckId)
    { NavigationManager.NavigateTo($"/Decks/{deckId}/Cards/New"); }

    private void PracticeDeck(long deckId)
    { NavigationManager.NavigateTo($"/Decks/{deckId}/Practice"); }
  }
}