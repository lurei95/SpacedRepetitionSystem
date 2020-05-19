using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System.Linq;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Utility.Extensions;
using SpacedRepetitionSystem.WebAPI.Validation.Core;
using SpacedRepetitionSystem.Entities.Validation.Decks;
using SpacedRepetitionSystem.Entities;

namespace SpacedRepetitionSystem.WebAPI.Validation.Decks
{
  /// <summary>
  /// CommitValidator for <see cref="Deck"/>
  /// </summary>
  public sealed class DeckCommitValidator : CommitValidatorBase<Deck>
  {
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="context">Context (Injected)</param>
    public DeckCommitValidator(DbContext context) : base(context) { }

    ///<inheritdoc/>
    public override string Validate(Deck entity)
    {
      string error = new DeckDefaultCardTemplateIdValidator().Validate(entity, entity.DefaultCardTemplateId);
      if (!string.IsNullOrEmpty(error))
        return error;
      error = new DeckTitleValidator().Validate(entity, entity.Title);
      if (!string.IsNullOrEmpty(error))
        return error;
      if (Context.Set<Deck>().Any(deck => deck.DeckId != entity.DeckId && deck.Title == entity.Title && deck.UserId == entity.UserId))
        return Errors.DeckTitleNotUnique.FormatWith(entity.Title);
      return null;
    }
  }
}