﻿using SpacedRepetitionSystem.Entities.Entities.Security;
using System;

namespace SpacedRepetitionSystem.Entities.Entities.Cards
{
  /// <summary>
  /// Entry of a practice history
  /// </summary>
  public sealed class PracticeHistoryEntry : IUserSpecificEntity, IRootEntity
  {
    ///<inheritdoc/>
    public object Id => PracticeHistoryEntryId;

    ///<inheritdoc/>
    public string Route => "PracticeHistoryEntries";

    /// <summary>
    /// Id of the history entry
    /// </summary>
    public long PracticeHistoryEntryId { get; set; }

    /// <summary>
    /// Date when it was practiced
    /// </summary>
    public DateTime PracticeDate { get; set; }

    /// <summary>
    /// Wrong count
    /// </summary>
    public int WrongCount { get; set; }

    /// <summary>
    /// Hard count
    /// </summary>
    public int HardCount { get; set; }

    /// <summary>
    /// Correct count
    /// </summary>
    public int CorrectCount { get; set; }

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
    /// The field
    /// </summary>
    public CardField Field { get; set; }

    /// <summary>
    /// Id of the card
    /// </summary>
    public long CardId { get; set; }

    /// <summary>
    /// Id of the deck the card belongs to
    /// </summary>
    public long DeckId { get; set; }

    /// <summary>
    /// Id of the field
    /// </summary>
    public int FieldId { get; set; }

    #endregion

    ///<inheritdoc/>
    public string GetDisplayName() => null;
  }
}