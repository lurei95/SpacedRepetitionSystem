using SpacedRepetitionSystem.Utility.Extensions;
using System.Collections.Generic;

namespace SpacedRepetitionSystem.Entities.Entities.SmartCards
{
  /// <summary>
  /// A Set of smart cards for practicing purpose
  /// </summary>
  public sealed class PracticeSet : IEntity
  {
    /// <summary>
    /// Id of the practice set
    /// </summary>
    public long PracticeSetId { get; set; }

    /// <summary>
    /// Title the practice set
    /// </summary>
    public string Title { get; set; }

    #region References

    /// <summary>
    /// The smart cards in the practice Set
    /// </summary>
    public List<SmartCard> SmartCards { get; } = new List<SmartCard>();

    /// <summary>
    /// Id of the default smart card definition for the set
    /// </summary>
    public long DefaultSmartCardDefinitionId { get; set; }

    /// <summary>
    /// The default smart card definition for the set
    /// </summary>
    public SmartCardDefinition DefaultSmartCardDefinition { get; set; }

    #endregion

    ///<inheritdoc>/>
    public string GetDisplayName() => EntityNames.PracticeSet.FormatWith(PracticeSetId); 
  }
}