using Microsoft.AspNetCore.Components;
using SpacedRepetitionSystem.Components.Commands;
using SpacedRepetitionSystem.Components.Middleware;
using SpacedRepetitionSystem.Components.ViewModels;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Utility.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.ViewModels.Cards
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
    public NavigationCommand ShowStatisticsCommand { get; private set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="navigationManager">NavigationManager (Injected)</param>
    /// <param name="apiConnector">ApiConnector (Injected)</param>
    public CardSearchViewModel(NavigationManager navigationManager, IApiConnector apiConnector) 
      : base(navigationManager, apiConnector)
    {
      ShowStatisticsCommand = new NavigationCommand(navigationManager)
      {
        CommandText = Messages.PracticeStatistics,
        IsRelative = true,
        TargetUriFactory = (param) =>  $"/Decks/{(param as Card).DeckId}/Cards/{(param as Card).CardId}/Statistics/"
      };
    }

    ///<inheritdoc/>
    public override async Task<bool> InitializeAsync()
    {
      bool result = await base.InitializeAsync();
      if (!result)
        return false;

      DeleteCommand.DeleteDialogTitle = Messages.DeleteCardDialogTitle;
      DeleteCommand.DeleteDialogTextFactory = (entity) => Messages.DeleteCardDialogText.FormatWith(entity.CardId);
      NewCommand.IsRelative = EditCommand.IsRelative = false;
      NewCommand.TargetUriFactory = (param) => $"/Decks/{SelectedEntity.DeckId}/Cards/New";
      EditCommand.TargetUriFactory = (param) => $"/Decks/{(param as Card).DeckId}/Cards/{(param as Card).CardId}";
      return true;
    }

    ///<inheritdoc/>
    protected override async Task<List<Card>> SearchCore()
    {
      Dictionary<string, object> parameters = new Dictionary<string, object>();
      if (DeckId.HasValue)
        parameters.Add(nameof(Deck.DeckId), DeckId);
      return (await ApiConnector.GetAsync<Card>(parameters)).Result;
    }
  }
}