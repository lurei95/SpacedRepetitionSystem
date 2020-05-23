using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using SpacedRepetitionSystem.Entities;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Entities.Validation.Cards;
using SpacedRepetitionSystem.WebAPI.Validation.Core;

namespace SpacedRepetitionSystem.WebAPI.Validation.Cards
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
      CardFieldValueValidator fieldValidator = new CardFieldValueValidator();
      foreach (CardField field in entity.Fields)
      {
        error = fieldValidator.Validate(field, field.Value);
        if (!string.IsNullOrEmpty(error))
          return error;
      }
      return null;
    }
  }
}