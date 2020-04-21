namespace SpacedRepetitionSystem.Entities.Entities.SmartCards
{
  /// <summary>
  /// Definition of a field of a smart card
  /// </summary>
  public sealed class SmartCardFieldDefinition : IEntity
  {
    /// <summary>
    /// Name of the field
    /// </summary>
    public string FieldName { get; set; }

    #region References

    /// <summary>
    /// Id of the smart card definition the field definition belongs to
    /// </summary>
    public long SmartCardDefinitionId { get; set; }

    /// <summary>
    /// The smart card definition the field definition belongs to
    /// </summary>
    public SmartCardDefinition SmartCardDefinition { get; set; }

    #endregion

    ///<inheritdoc>/>
    public string GetDisplayName() => null;
  }
}