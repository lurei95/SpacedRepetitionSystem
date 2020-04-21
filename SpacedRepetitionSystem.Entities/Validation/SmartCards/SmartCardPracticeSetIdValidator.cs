using SpacedRepetitionSystem.Entities.Entities.SmartCards;
using SpacedRepetitionSystem.Entities.Validation.Core;
using SpacedRepetitionSystem.Utility.Extensions;

namespace SpacedRepetitionSystem.Entities.Validation.SmartCards
{
  /// <summary>
  /// Validator for <see cref="SmartCard.PracticeSetId"/>
  /// </summary>
  public sealed class SmartCardPracticeSetIdValidator : PropertyValidatorBase<SmartCard, long>
  {
    ///<inheritdoc/>
    public override string PropertyName => nameof(SmartCard.PracticeSetId);

    ///<inheritdoc/>
    public override string Validate(SmartCard entity, long newValue)
    {
      if (newValue == default)
        return Errors.PropertyRequired.FormatWith(PropertyNames.PracticeSet);
      return null;
    }
  }
}