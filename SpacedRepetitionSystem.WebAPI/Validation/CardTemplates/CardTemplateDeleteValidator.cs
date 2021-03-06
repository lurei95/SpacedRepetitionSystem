﻿using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using SpacedRepetitionSystem.Entities;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Utility.Extensions;
using SpacedRepetitionSystem.WebAPI.Validation.Core;

namespace SpacedRepetitionSystem.WebAPI.Validation.CardTemplates
{
  /// <summary>
  /// DeleteValidator for <see cref="CardTemplate"/>
  /// </summary>
  public sealed class CardTemplateDeleteValidator : DeleteValidatorBase<CardTemplate>
  {
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="context">Context (injected)</param>
    public CardTemplateDeleteValidator(DbContext context) : base(context) { }

    ///<inheritdoc/>
    public override string Validate(CardTemplate entity)
    {
      if (Context.Set<Deck>().AsNoTracking().Any(deck => deck.DefaultCardTemplateId == entity.CardTemplateId)
        || Context.Set<Card>().AsNoTracking().Any(card => card.CardTemplateId == entity.CardTemplateId))
        return Errors.CardTemplateInUse.FormatWith(entity.Title);
      return null;
    }
  }
}