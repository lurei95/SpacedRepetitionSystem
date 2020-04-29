using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Entities.Validation.Core;
using SpacedRepetitionSystem.Utility.Extensions;

namespace SpacedRepetitionSystem.Entities.Validation.Decks
{
  /// <summary>
  /// Validator for <see cref="Deck.Title"/>
  /// </summary>
  public sealed class DeckTitleValidator : PropertyValidatorBase<Deck, string>
  {
    ///<inheritdoc/>
    public override string PropertyName => nameof(Deck.Title);

    ///<inheritdoc/>
    public override string Validate(Deck entity, string newValue)
    {
      if (string.IsNullOrEmpty(newValue))
        return Errors.PropertyRequired.FormatWith(PropertyNames.Title);
      return null;
    }
  }
}