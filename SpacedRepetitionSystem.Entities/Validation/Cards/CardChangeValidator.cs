using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Entities.Validation.Core;

namespace SpacedRepetitionSystem.Entities.Validation.Cards
{
  /// <summary>
  /// Validator for validating the changes to <see cref="Card"/>
  /// </summary>
  public sealed class CardChangeValidator : EntityChangeValidator<Card>
  {
    private readonly EntityChangeValidator<CardField> fieldChangeValidator;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="fieldChangeValidator">FieldChangeValidator (Injected)</param>
    public CardChangeValidator(EntityChangeValidator<CardField> fieldChangeValidator)
    { this.fieldChangeValidator = fieldChangeValidator; }

    /// <summary>
    /// Validates the new value of the property
    /// </summary>
    /// <typeparam name="TProperty">Proeprty-Type</typeparam>
    /// <param name="propertyName">Name of the property</param>
    /// <param name="entity">The entity</param>
    /// <param name="newValue">The new value of the property</param>
    /// <returns>Error message or null</returns>
    public override string Validate<TProperty>(string propertyName, object entity, TProperty newValue)
    {
      if (entity is CardField)
        return fieldChangeValidator.Validate(propertyName, entity, newValue);
      else
        return base.Validate(propertyName, entity, newValue);
    }
  }
}