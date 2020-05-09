using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using SpacedRepetitionSystem.Components.Commands;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Logic.Controllers.Core;
using SpacedRepetitionSystem.Utility.Dialogs;
using SpacedRepetitionSystem.Utility.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Components.ViewModels.Cards
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
    }

    ///<inheritdoc/>
    protected override void DeleteEntity(Deck entity)
    {
      ModalDialogManager.ShowDialog(Messages.DeleteDeckDialogTitle, 
        Messages.DeleteDeckDialogText.FormatWith(entity.Title), DialogButtons.YesNo, (result) =>
      {
        if (result == DialogResult.Yes)
          base.DeleteEntity(entity);
      });
    }

    ///<inheritdoc/>
    protected override async Task<List<Deck>> SearchCore() => await ApiConnector.Get<Deck>(null);

    private void AddCards(long deckId)
    { NavigationManager.NavigateTo($"/Decks/{deckId}/Cards/New"); }

    private void PracticeDeck(long deckId)
    { NavigationManager.NavigateTo($"/Decks/{deckId}/Practice"); }
  }
}