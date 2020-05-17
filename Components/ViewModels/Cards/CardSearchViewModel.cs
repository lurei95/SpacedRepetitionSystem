using Microsoft.AspNetCore.Components;
using SpacedRepetitionSystem.Components.Commands;
using SpacedRepetitionSystem.Components.Middleware;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Utility.Dialogs;
using SpacedRepetitionSystem.Utility.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Components.ViewModels.Cards
{
  /// <summary>
  /// SearchViewModel for <see cref="Card"/>
  /// </summary>
  public sealed class CardSearchViewModel : SearchViewModelBase<Card>
  {
    /// <summary>
    /// Id of the deck
    /// </summary>
    public long? DeckId { get; set; }

    ///<inheritdoc/>
    public override Card SelectedEntity 
    { 
      get => base.SelectedEntity; 
      set
      { 
        base.SelectedEntity = value;
        NewCommand.IsEnabled = SelectedEntity != null;
      }
    }

    /// <summary>
    /// Command for showing the practice statistics
    /// </summary>
    public Command ShowStatisticsCommand { get; private set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="navigationManager">NavigationManager (Injected)</param>
    /// <param name="apiConnector">ApiConnector (Injected)</param>
    public CardSearchViewModel(NavigationManager navigationManager, IApiConnector apiConnector) 
      : base(navigationManager, apiConnector)
    {
      ShowStatisticsCommand = new Command()
      {
        CommandText = Messages.PracticeStatistics,
        ExecuteAction = (param) => ShowStatistics(param as Card)
      };
    }

    ///<inheritdoc/>
    protected override async Task<List<Card>> SearchCore()
    {
      Dictionary<string, object> parameters = new Dictionary<string, object>();
      if (DeckId.HasValue)
        parameters.Add(nameof(Deck.DeckId), DeckId);
      return await ApiConnector.GetAsync<Card>(parameters);
    }

    ///<inheritdoc/>
    protected override async Task DeleteEntity(Card entity)
    {
      ModalDialogManager.ShowDialog(Messages.DeleteCardDialogTitle,
        Messages.DeleteCardDialogText.FormatWith(entity.CardId), DialogButtons.YesNo, async (result) =>
      {
        if (result == DialogResult.Yes)
          await base.DeleteEntity(entity);
      });
    }

    ///<inheritdoc/>
    protected override void NewEntity()
    { NavigationManager.NavigateTo("/Decks/" + SelectedEntity.DeckId + "/Cards/New"); }

    ///<inheritdoc/>
    protected override void EditEntity(Card entity)
    { NavigationManager.NavigateTo("/Decks/" + entity.DeckId + "/Cards/" + entity.CardId); }

    ///<inheritdoc/>
    private void ShowStatistics(Card card)
    { NavigationManager.NavigateTo("/Decks/" + card.DeckId + "/Cards/" + card.CardId + "/Statistics/"); }
  }
}
