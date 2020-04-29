using Microsoft.EntityFrameworkCore;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Entities.Validation.Core;

namespace SpacedRepetitionSystem.Entities.Validation.Cards
{
  /// <summary>
  /// CommitValidator for <see cref="Card"/>
  /// </summary>
  public sealed class CardCommitValidator : CommitValidatorBase<Card>
  {
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="context">Context (Injected)</param>
    public CardCommitValidator(DbContext context) : base(context) { }

    ///<inheritdoc/>
    public override string Validate(Card entity)
    {
      string error = new CardCardTemplateIdValidator().Validate(entity, entity.CardTemplateId);
      if (!string.IsNullOrEmpty(error))
        return error;
      error = new CardDeckIdValidator().Validate(entity, entity.DeckId);
      if (!string.IsNullOrEmpty(error))
        return error;
      return null;
    }
  }
}