using System;

namespace SpacedRepetitionSystem.Entities.Entities.Cards
{
  /// <summary>
  /// A field for practicing
  /// </summary>
  public sealed class PracticeField : IEntity
  {
    /// <summary>
    /// Id of the deck
    /// </summary>
    public long DeckId { get; set; }

    /// <summary>
    /// The Deck
    /// </summary>
    public Deck Deck { get; set; }

    /// <summary>
    /// Id of the Card
    /// </summary>
    public long CardId { get; set; }

    /// <summary>
    /// Name of the field
    /// </summary>
    public string FieldName { get; set; }

    /// <summary>
    /// The field
    /// </summary>
    public CardField Field { get; set; }

    /// <summary>
    /// The next due date for practicing the field
    /// </summary>
    public DateTime DueDate { get; set; }

    ///<inheritdoc/>
    public string GetDisplayName() => null;
  }
}
