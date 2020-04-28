namespace SpacedRepetitionSystem.Entities.Entities.Cards
{
  /// <summary>
  /// Definition of a field of a card
  /// </summary>
  public sealed class CardFieldDefinition : IEntity
  {
    /// <summary>
    /// Name of the field
    /// </summary>
    public string FieldName { get; set; }

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

    ///<inheritdoc>/>
    public string GetDisplayName() => null;
  }
}