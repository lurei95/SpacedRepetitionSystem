using SpacedRepetitionSystem.Entities.Entities.Security;
using System;

namespace SpacedRepetitionSystem.Entities.Entities.Cards
{
  /// <summary>
  /// A Field of a card
  /// </summary>
  public sealed class CardField : IUserSpecificEntity
  {
    ///<inheritdoc/>
    public object Id => new { CardId, FieldName };

    /// <summary>
    /// Value of the field
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// The next due date for practicing the field
    /// </summary>
    public DateTime DueDate { get; set; } = DateTime.Today;

    /// <summary>
    /// Level of proficiency
    /// </summary>
    public int ProficiencyLevel { get; set; } = 1;

    /// <summary>
    /// Returns whether the practice field is due
    /// </summary>
    public bool IsDue => DueDate <= DateTime.Today;

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
    /// Id of the card the field belongs to
    /// </summary>
    public long CardId { get; set; }

    /// <summary>
    /// The card the field belongs to
    /// </summary>
    public Card Card { get; set; }

    /// <summary>
    /// Id of the template of the card the field belongs to
    /// </summary>
    public long CardTemplateId { get; set; }

    /// <summary>
    /// The template of the card the field belongs to
    /// </summary>
    public CardTemplate CardTemplate { get; set; }

    /// <summary>
    /// Name of the field
    /// </summary>
    public string FieldName { get; set; }

    /// <summary>
    /// The field definition of the field
    /// </summary>
    public CardFieldDefinition CardFieldDefinition { get; set; }

    #endregion

    ///<inheritdoc>/>
    public string GetDisplayName() => null;
  }
}