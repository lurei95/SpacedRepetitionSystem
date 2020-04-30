﻿using Microsoft.EntityFrameworkCore;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Entities.Validation.Core;

namespace SpacedRepetitionSystem.Entities.Validation.CardTemplates
{
  /// <summary>
  /// CommitValidator for <see cref="CardTemplate"/>
  /// </summary>
  public sealed class CardTemplateCommitValidator : CommitValidatorBase<CardTemplate>
  {
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="context">Context (Injected)</param>
    public CardTemplateCommitValidator(DbContext context) : base(context) { }

    ///<inheritdoc/>
    public override string Validate(CardTemplate entity)
    {
      string error = new CardTemplateTitleValidator().Validate(entity, entity.Title);
      if (!string.IsNullOrEmpty(error))
        return error;
      return null;
    }
  }
}