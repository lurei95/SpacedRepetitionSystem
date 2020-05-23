using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Entities.Validation.Core;
using SpacedRepetitionSystem.Utility.Extensions;

namespace SpacedRepetitionSystem.Entities.Validation.Cards
{
  /// <summary>
  /// Validator for <see cref="Card.DeckId"/>
  /// </summary>
  public sealed class CardDeckIdValidator : PropertyValidatorBase<Card, long>
  {
    ///<inheritdoc/>
    public override string PropertyName => nameof(Card.DeckId);

    ///<inheritdoc/>
    public override string Validate(Card entity, long newValue)
    {
      if (newValue == default)
        return Errors.PropertyRequired.FormatWith(EntityNameHelper.GetName<Deck>());
      return null;
    }
  }
}