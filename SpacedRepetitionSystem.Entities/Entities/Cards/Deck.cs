using SpacedRepetitionSystem.Entities.Entities.Security;
using SpacedRepetitionSystem.Utility.Extensions;
using System;
using System.Collections.Generic;

namespace SpacedRepetitionSystem.Entities.Entities.Cards
{
  /// <summary>
  /// A Set of cards for practicing purpose
  /// </summary>
  public sealed class Deck : IUserSpecificEntity, IRootEntity
  {
    ///<inheritdoc/>
    public object Id => DeckId;

    ///<inheritdoc/>
    public string Route => "Decks";

    /// <summary>
    /// Id of the deck
    /// </summary>
    public long DeckId { get; set; }

    /// <summary>
    /// Title the deck
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Whether the deck is pinned to be shon in the home page
    /// </summary>
    public bool IsPinned { get; set; }

    #region References

    /// <summary>
    /// Id of the user
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// User
    /// </summary>
    public User User { get; set; }

    /// <summary>
    /// The cards in the deck
    /// </summary>
    public List<Card> Cards { get; } = new List<Card>();

    /// <summary>
    /// Count of the Cards in the deck
    /// </summary>
    public int CardCount { get; set; }

    /// <summary>
    /// Count of the Cards in the deck that are due
    /// </summary>
    public int DueCardCount { get; set; }

    /// <summary>
    /// Id of the default card template for the deck
    /// </summary>
    public long DefaultCardTemplateId { get; set; }

    /// <summary>
    /// The default card template for the deck
    /// </summary>
    public CardTemplate DefaultCardTemplate { get; set; }

    #endregion

    ///<inheritdoc/>
    public string GetDisplayName() => EntityNames.Deck.FormatWith(Title); 
  }
}