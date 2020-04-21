using System;

namespace SpacedRepetitionSystem.Entities.Entities.SmartCards
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
    /// Id of the smart card
    /// </summary>
    public long SmartCardId { get; set; }

    /// <summary>
    /// The smart card
    /// </summary>
    public SmartCard SmartCard { get; set; }

    /// <summary>
    /// Id of the field of the smart card practiced
    /// </summary>
    public long SmartCardFieldDefinitionId { get; set; }

    /// <summary>
    /// The field of the smart card practiced
    /// </summary>
    public SmartCardDefinition SmartCardDefinition { get; set; }

    #endregion

    ///<inheritdoc>/>
    public string GetDisplayName() => null;
  }
}