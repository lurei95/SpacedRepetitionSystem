namespace SpacedRepetitionSystem.Entities.Entities.Cards
{
  /// <summary>
  /// Definition of a field of a card
  /// </summary>
  public sealed class CardFieldDefinition : IEntity
  {
    ///<inheritdoc/>
    public object Id => new { CardTemplateId, FieldName };

    /// <summary>
    /// Name of the field
    /// </summary>
    public string FieldName { get; set; }

    /// <summary>
    /// Whether the input should be used for praticing purpose
    /// </summary>
    public bool ShowInputForPractice { get; set; } = false;

    #region References

    /// <summary>
    /// Id of the card template the field definition belongs to
    /// </summary>
    public long CardTemplateId { get; set; }

    /// <summary>
    /// The card template the field definition belongs to
    /// </summary>
    public CardTemplate CardTemplate { get; set; }

    #endregion

    ///<inheritdoc/>
    public string GetDisplayName() => null;
  }
}