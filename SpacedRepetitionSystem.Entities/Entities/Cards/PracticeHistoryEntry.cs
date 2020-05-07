using System;

namespace SpacedRepetitionSystem.Entities.Entities.Cards
{
  /// <summary>
  /// Entry of a practice history
  /// </summary>
  public sealed class PracticeHistoryEntry : IEntity
  {
    ///<inheritdoc/>
    public object Id => new { DeckId, CardId, FieldName };

    /// <summary>
    /// Id of the deck
    /// </summary>
    public long DeckId { get; set; }

    /// <summary>
    /// Id of the card
    /// </summary>
    public long CardId { get; set; }

    /// <summary>
    /// Name of the field
    /// </summary>
    public string FieldName { get; set; }

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

    ///<inheritdoc/>
    public string GetDisplayName() => null;
  }
}