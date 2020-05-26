using Microsoft.AspNetCore.Components;
using SpacedRepetitionSystem.Components.Commands;
using SpacedRepetitionSystem.Components.Middleware;
using SpacedRepetitionSystem.Components.ViewModels;
using SpacedRepetitionSystem.Entities;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Utility.Extensions;
using SpacedRepetitionSystem.Utility.Notification;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.ViewModels.Cards
{
  /// <summary>
  /// SearchViewModel for <see cref="Card"/>
  /// </summary>
  public sealed class CardSearchViewModel : SearchViewModelBase<Card>
  {
    private string selectedDeckTitle = null;
    private Dictionary<string, Deck> availableDecks = new Dictionary<string, Deck>();
    private static readonly string AllDecks = Messages.All;

    /// <summary>
    /// The available card templates
    /// </summary>
    public List<string> AvailableDecks => availableDecks.Keys.ToList();

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
    /// Whether the deck is selctable
    /// </summary>
    public bool DeckSelectable { get; set; } = true;

    /// <summary>
    /// The selected deck title
    /// </summary>
    public string SelectedDeckTitle
    {
      get => selectedDeckTitle;
      set
      {
        if (value != selectedDeckTitle)
        {
          selectedDeckTitle = value;
          if (string.IsNullOrEmpty(value) || value == AllDecks)
            DeckId = null;
          else
            DeckId = availableDecks[value].DeckId;
          SearchAsync();
        }
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
        ToolTip = Messages.ShowStatisticsCommandToolTip.FormatWith(EntityNameHelper.GetName<Card>()),
        IsRelative = true,
        TargetUriFactory = (param) =>  $"/Decks/{(param as Card).DeckId}/Cards/{(param as Card).CardId}/Statistics"
      };
    }

    ///<inheritdoc/>
    public override async Task<bool> InitializeAsync()
    {
      bool result = await LoadAvaliableDecksAsync();
      if (!result)
        return result;

      result = await base.InitializeAsync();
      if (!result)
        return result;

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
      if (!string.IsNullOrEmpty(SearchText))
        parameters.Add(nameof(SearchText), SearchText);
      return (await ApiConnector.GetAsync<Card>(parameters)).Result;
    }

    private async Task<bool> LoadAvaliableDecksAsync()
    {
      ApiReply<List<Deck>> reply = await ApiConnector.GetAsync<Deck>(new Dictionary<string, object>());
      if (!reply.WasSuccessful)
      {
        NotificationMessageProvider.ShowErrorMessage(reply.ResultMessage);
        return false;
      }
      availableDecks.Add(AllDecks, null);
      foreach (Deck deck in reply.Result)
        availableDecks.Add(deck.Title, deck);
      if (DeckId.HasValue)
      {
        DeckSelectable = false;
        selectedDeckTitle = availableDecks
          .SingleOrDefault(pair => pair.Value != null && pair.Value.DeckId == DeckId).Key;
      }
      return true;
    }
  }
}