using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Entities.Validation.Core;
using SpacedRepetitionSystem.Utility.Extensions;

namespace SpacedRepetitionSystem.Entities.Validation.Cards
{
  /// <summary>
  /// Validator for <see cref="CardTemplate.Title"/>
  /// </summary>
  public sealed class CardTemplateTitleValidator : PropertyValidatorBase<CardTemplate, string>
  {
    ///<inheritdoc/>
    public override string PropertyName => nameof(CardTemplate.Title);

    ///<inheritdoc/>
    public override string Validate(CardTemplate entity, string newValue)
    {
      if (string.IsNullOrEmpty(newValue))
        return Errors.PropertyRequired.FormatWith(PropertyNames.Title);
      return null;
    }
  }
}