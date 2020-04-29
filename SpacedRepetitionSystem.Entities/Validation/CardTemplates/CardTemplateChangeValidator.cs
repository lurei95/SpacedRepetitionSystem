using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Entities.Validation.Core;

namespace SpacedRepetitionSystem.Entities.Validation.CardTemplates
{
  /// <summary>
  /// Validator that validates the change of <see cref="CardTemplate"/>
  /// </summary>
  public class CardTemplateChangeValidator : EntityChangeValidator<CardTemplate>
  {
    private readonly EntityChangeValidator<CardFieldDefinition> fieldDefinitionChangeValidator;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="fieldDefinitionChangeValidator">FieldDefinitionChangeValidator (Injected)</param>
    public CardTemplateChangeValidator(EntityChangeValidator<CardFieldDefinition> fieldDefinitionChangeValidator)
    { this.fieldDefinitionChangeValidator = fieldDefinitionChangeValidator;  }

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
      if (entity is CardFieldDefinition)
        return fieldDefinitionChangeValidator.Validate(propertyName, entity, newValue);
      else
        return base.Validate(propertyName, entity, newValue);
    }
  }
}