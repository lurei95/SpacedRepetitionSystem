using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Entities.Validation.Core;

namespace SpacedRepetitionSystem.Entities.Validation.Cards
{
  /// <summary>
  /// Validator for <see cref="CardField.Value"/>
  /// </summary>
  public sealed class CardFieldValueValidator : PropertyValidatorBase<CardField, string>
  {
    ///<inheritdoc/>
    public override string PropertyName => nameof(CardField.Value);

    ///<inheritdoc/>
    public override string Validate(CardField entity, string newValue)
    {
      if (string.IsNullOrEmpty(newValue) && entity.CardFieldDefinition.IsRequired)
        return Errors.FieldRequiresValue;
      return null;
    }
  }
}