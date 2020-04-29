using Microsoft.EntityFrameworkCore;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Entities.Validation.Core;

namespace SpacedRepetitionSystem.Entities.Validation.Decks
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
      return null;
    }
  }
}