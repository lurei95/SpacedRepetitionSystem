using SpacedRepetitionSystem.Entities.Entities.Security;
using SpacedRepetitionSystem.Utility.Extensions;
using System;
using System.Collections.Generic;

namespace SpacedRepetitionSystem.Entities.Entities.Cards
{
  /// <summary>
  /// A card
  /// </summary>
  public sealed class Card : IUserSpecificEntity, IRootEntity
  {
    ///<inheritdoc/>
    public object Id => CardId;

    ///<inheritdoc/>
    public string Route => "Cards";

    /// <summary>
    /// Id of the card
    /// </summary>
    public long CardId { get; set; }

    /// <summary>
    /// The tags the card is amrked with
    /// </summary>
    public string Tags { get; set; } = string.Empty;

    #region References

    /// <summary>
    /// Id of the user
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// User
    /// </summary>
    public User User { get; set; }

    /// <summary>
    /// Id of the template of the card
    /// </summary>
    public long CardTemplateId { get; set; }

    /// <summary>
    /// The template of the card
    /// </summary>
    public CardTemplate CardTemplate { get; set; }

    /// <summary>
    /// Id of the deck the card belongs to
    /// </summary>
    public long DeckId { get; set; }

    /// <summary>
    /// The deck card belongs to
    /// </summary>
    public Deck Deck { get; set; }

    /// <summary>
    /// The fields of the card
    /// </summary>
    public List<CardField> Fields { get; } = new List<CardField>();

    #endregion

    ///<inheritdoc/>
    public string GetDisplayName() 
      => EntityNames.EntityDisplayNameFormat.FormatWith(EntityNameHelper.GetName<Card>(), CardId);
  }
}