using Microsoft.AspNetCore.Components;
using SpacedRepetitionSystem.Components.Commands;
using SpacedRepetitionSystem.Components.Middleware;
using SpacedRepetitionSystem.Components.ViewModels;
using SpacedRepetitionSystem.Entities;
using SpacedRepetitionSystem.Entities.Entities.Cards;
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
    /// Constructor
    /// </summary>
    /// <param name="navigationManager">NavigationManager (Injected)</param>
    /// <param name="apiConnector">ApiConnector (Injected)</param>
    public DeckSearchViewModel(NavigationManager navigationManager, IApiConnector apiConnector) 
      : base(navigationManager, apiConnector)
    {
      PracticeDeckCommand = new NavigationCommand(navigationManager)
      {
        CommandText = Messages.Practice,
        IsRelative = true,
        ToolTip = Messages.PracticeCommandToolTip.FormatWith(EntityNameHelper.GetName<Deck>()),
        TargetUriFactory = (param) => $"/{(long)param}/Practice"
      };
      AddCardCommand = new NavigationCommand(navigationManager)
      {
        CommandText = Messages.NewCard,
        IsRelative = true,
        ToolTip = Components.Messages.NewCommandToolTip.FormatWith(EntityNameHelper.GetName<Card>()),
        TargetUriFactory = (param) => $"/{(long)param}/Cards/New"
      };
      ShowStatisticsCommand = new NavigationCommand(navigationManager)
      {
        CommandText = Messages.PracticeStatistics,
        IsRelative = true,
        ToolTip = Messages.ShowStatisticsCommandToolTip.FormatWith(EntityNameHelper.GetName<Deck>()),
        TargetUriFactory = (param) => $"/{(param as Deck).DeckId}/Statistics"
      };
    }

    ///<inheritdoc/>
    public override async Task<bool> InitializeAsync()
    {
      bool result = await base.InitializeAsync();
      if (!result)
        return false;

      DeleteCommand.DeleteDialogTitle = Messages.DeleteDeckDialogTitle;
      DeleteCommand.DeleteDialogTextFactory = (entity) => Messages.DeleteDeckDialogText.FormatWith(entity.Title);
      return true;
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
    protected override async Task<ApiReply<List<Deck>>> SearchCore()
    {
      Dictionary<string, object> parameters = new Dictionary<string, object>();
      if (!string.IsNullOrEmpty(SearchText))
        parameters.Add(nameof(SearchText), SearchText);
      return await ApiConnector.GetAsync<Deck>(parameters);
    }
  }
}