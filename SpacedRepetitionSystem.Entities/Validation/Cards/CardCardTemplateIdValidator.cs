using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Entities.Validation.Core;
using SpacedRepetitionSystem.Utility.Extensions;

namespace SpacedRepetitionSystem.Entities.Validation.Cards
{
  /// <summary>
  /// Validator for <see cref="Card.CardTemplateId"/>
  /// </summary>
  public sealed class CardCardTemplateIdValidator : PropertyValidatorBase<Card, long>
  {
    ///<inheritdoc/>
    public override string PropertyName => nameof(Card.CardTemplateId);

    ///<inheritdoc/>
    public override string Validate(Card entity, long newValue)
    {
      if (newValue == default)
        return Errors.PropertyRequired.FormatWith(EntityNameHelper.GetName<CardTemplate>());
      return null;
    }
  }
}