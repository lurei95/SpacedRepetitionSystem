namespace SpacedRepetitionSystem.Entities.Entities.SmartCards
{
  /// <summary>
  /// A Field of a smart card
  /// </summary>
  public sealed class SmartCardField : IEntity
  {
    /// <summary>
    /// Value of the field
    /// </summary>
    public string Value { get; set; }

    #region References

    /// <summary>
    /// Id of the smart card the field belongs to
    /// </summary>
    public long SmartCardId { get; set; }

    /// <summary>
    /// The smart card the field belongs to
    /// </summary>
    public SmartCard SmartCard { get; set; }

    /// <summary>
    /// Id of the smart card definition the field belongs to
    /// </summary>
    public long SmartCardDefinitionId { get; set; }

    /// <summary>
    /// The smart card definition the field belongs to
    /// </summary>
    public SmartCardDefinition SmartCardDefinition { get; set; }

    /// <summary>
    /// Name of the field
    /// </summary>
    public string FieldName { get; set; }

    /// <summary>
    /// The field definition of the field
    /// </summary>
    public SmartCardFieldDefinition SmartCardFieldDefinition { get; set; }

    #endregion

    ///<inheritdoc>/>
    public string GetDisplayName() => null;
  }
}