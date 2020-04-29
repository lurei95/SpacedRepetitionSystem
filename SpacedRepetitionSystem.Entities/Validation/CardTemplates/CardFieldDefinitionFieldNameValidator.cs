using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Entities.Validation.Core;
using SpacedRepetitionSystem.Utility.Extensions;

namespace SpacedRepetitionSystem.Entities.Validation.CardTemplates
{
  /// <summary>
  /// Validator for <see cref="CardFieldDefinition.FieldName"/>
  /// </summary>
  public sealed class CardFieldDefinitionFieldNameValidator : PropertyValidatorBase<CardFieldDefinition, string>
  {
    ///<inheritdoc/>
    public override string PropertyName => nameof(CardFieldDefinition.FieldName);

    ///<inheritdoc/>
    public override string Validate(CardFieldDefinition entity, string newValue)
    {
      if (string.IsNullOrEmpty(newValue))
        return Errors.PropertyRequired.FormatWith(PropertyNames.Title);
      return null;
    }
  }
}