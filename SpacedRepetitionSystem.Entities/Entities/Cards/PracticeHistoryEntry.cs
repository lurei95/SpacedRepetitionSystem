using System;

namespace SpacedRepetitionSystem.Entities.Entities.Cards
{
  /// <summary>
  /// History entry for a practice result
  /// </summary>
  public sealed class PracticeHistoryEntry : IEntity
  {
    /// <summary>
    /// Date of the history entry
    /// </summary>
    public DateTime PracticeDate { get; set; }

    /// <summary>
    /// Result of the practice attempt
    /// </summary>
    public PracticeResultKind PracticeResult { get; set; }

    #region References

    /// <summary>
    /// Id of the  card
    /// </summary>
    public long CardId { get; set; }

    /// <summary>
    /// The card
    /// </summary>
    public Card Card { get; set; }

    /// <summary>
    /// Id of the field of the card practiced
    /// </summary>
    public long CardFieldDefinitionId { get; set; }

    /// <summary>
    /// The field of the card practiced
    /// </summary>
    public CardTemplate CardTemplate { get; set; }

    #endregion

    ///<inheritdoc>/>
    public string GetDisplayName() => null;
  }
}