using SpacedRepetitionSystem.Utility.Extensions;
using System.Collections.Generic;

namespace SpacedRepetitionSystem.Entities.Entities.SmartCards
{
  /// <summary>
  /// Definition of smart card
  /// </summary>
  public sealed class SmartCardDefinition : IEntity
  {
    /// <summary>
    /// The id of the default smart card definition
    /// </summary>
    public static long DefaultSmartCardDefinitionId => 1;

    /// <summary>
    /// The Id of the definition of the smart card
    /// </summary>
    public long SmartCardDefinitionId { get; set; }

    /// <summary>
    /// Title of the smart card definition
    /// </summary>
    public string Title { get; set; }

    #region References

    /// <summary>
    /// The field definitions of the smart card definition
    /// </summary>
    public List<SmartCardFieldDefinition> FieldDefinitions { get; } = new List<SmartCardFieldDefinition>();

    #endregion

    ///<inheritdoc>/>
    public string GetDisplayName() => EntityNames.SmartCardDefinition.FormatWith(SmartCardDefinitionId);
  }
}