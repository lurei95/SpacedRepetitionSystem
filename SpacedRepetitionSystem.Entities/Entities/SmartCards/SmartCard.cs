using SpacedRepetitionSystem.Utility.Extensions;
using System.Collections.Generic;

namespace SpacedRepetitionSystem.Entities.Entities.SmartCards
{
  /// <summary>
  /// A smart card
  /// </summary>
  public sealed class SmartCard : IEntity
  {
    /// <summary>
    /// Id of the smart card
    /// </summary>
    public long SmartCardId { get; set; }

    /// <summary>
    /// The tags the smart card is amrked with
    /// </summary>
    public string Tags { get; set; } = string.Empty;

    #region References

    /// <summary>
    /// Id of the definition of the smart card
    /// </summary>
    public long SmartCardDefinitionId { get; set; }

    /// <summary>
    /// The definition of the smart card
    /// </summary>
    public SmartCardDefinition SmartCardDefinition { get; set; }

    /// <summary>
    /// Id of the set the smart card belongs to
    /// </summary>
    public long PracticeSetId { get; set; }

    /// <summary>
    /// The set the smart card belongs to
    /// </summary>
    public PracticeSet PracticeSet { get; set; }

    /// <summary>
    /// The fields of the smart card
    /// </summary>
    public List<SmartCardField> Fields { get; } = new List<SmartCardField>();

    #endregion

    ///<inheritdoc>/>
    public string GetDisplayName() => EntityNames.SmartCard.FormatWith(SmartCardId);
  }
}