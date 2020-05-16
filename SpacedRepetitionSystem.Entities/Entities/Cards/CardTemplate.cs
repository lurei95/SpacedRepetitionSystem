using SpacedRepetitionSystem.Entities.Entities.Security;
using SpacedRepetitionSystem.Utility.Extensions;
using System.Collections.Generic;

namespace SpacedRepetitionSystem.Entities.Entities.Cards
{
  /// <summary>
  /// Template for cards
  /// </summary>
  public sealed class CardTemplate : IUserSpecificEntity
  {
    ///<inheritdoc/>
    public object Id => CardTemplateId;

    /// <summary>
    /// The id of the default template
    /// </summary>
    public static long DefaultCardTemplateId => 1;

    /// <summary>
    /// The Id of the template
    /// </summary>
    public long CardTemplateId { get; set; }

    /// <summary>
    /// Title of the template
    /// </summary>
    public string Title { get; set; }

    #region References

    /// <summary>
    /// Id of the user
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// User
    /// </summary>
    public User User { get; set; }

    /// <summary>
    /// The field definitions of the template
    /// </summary>
    public List<CardFieldDefinition> FieldDefinitions { get; } = new List<CardFieldDefinition>();

    #endregion

    ///<inheritdoc>/>
    public string GetDisplayName() => EntityNames.CardTemplate.FormatWith(Title);
  }
}