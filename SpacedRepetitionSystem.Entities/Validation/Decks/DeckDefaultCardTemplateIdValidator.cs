using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Entities.Validation.Core;
using SpacedRepetitionSystem.Utility.Extensions;

namespace SpacedRepetitionSystem.Entities.Validation.Decks
{
  /// <summary>
  /// Validator for <see cref="Deck.DefaultCardTemplateId"/>
  /// </summary>
  public sealed class DeckDefaultCardTemplateIdValidator : PropertyValidatorBase<Deck, long>
  {
    ///<inheritdoc/>
    public override string PropertyName => nameof(Deck.DefaultCardTemplateId);

    ///<inheritdoc/>
    public override string Validate(Deck entity, long newValue)
    {
      if (newValue == default)
        return Errors.PropertyRequired.FormatWith(PropertyNames.DefaultCardTemplate);
      return null;
    }
  }
}