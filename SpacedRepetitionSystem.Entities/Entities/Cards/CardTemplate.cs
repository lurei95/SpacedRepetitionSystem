﻿using SpacedRepetitionSystem.Entities.Entities.Security;
using SpacedRepetitionSystem.Utility.Extensions;
using System;
using System.Collections.Generic;

namespace SpacedRepetitionSystem.Entities.Entities.Cards
{
  /// <summary>
  /// Template for cards
  /// </summary>
  public sealed class CardTemplate : IUserSpecificEntity, IRootEntity
  {
    ///<inheritdoc/>
    public object Id => CardTemplateId;

    ///<inheritdoc/>
    public string Route => "CardTemplates";

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
    public Guid UserId { get; set; }

    /// <summary>
    /// User
    /// </summary>
    public User User { get; set; }

    /// <summary>
    /// The field definitions of the template
    /// </summary>
    public List<CardFieldDefinition> FieldDefinitions { get; } = new List<CardFieldDefinition>();

    #endregion

    ///<inheritdoc/>
    ///<inheritdoc/>
    public string GetDisplayName()
      => EntityNames.EntityDisplayNameFormat.FormatWith(EntityNameHelper.GetName<CardTemplate>(), Title);
  }
}