using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Entities.Validation.CardTemplates;
using SpacedRepetitionSystem.Entities.Validation.Core;

namespace SpacedRepetitionSystem.Entities.Tests.Validation.CardTemplates
{
  /// <summary>
  /// Testclass for <see cref="CardTemplateChangeValidator"/>
  /// </summary>
  [TestClass]
  public sealed class CardTemplateChangeValidatorTests
  {
    /// <summary>
    /// Tests that <see cref="CardFieldDefinition.FieldName"/> is validated
    /// </summary>
    [TestMethod]
    public void ValidateTitleTest()
    {
      EntityChangeValidator<CardFieldDefinition> changeValidator = new EntityChangeValidator<CardFieldDefinition>();
      changeValidator.Register(nameof(CardFieldDefinition.FieldName), new CardFieldDefinitionFieldNameValidator());
      CardTemplateChangeValidator validator = new CardTemplateChangeValidator(changeValidator);
      validator.Register(nameof(CardTemplate.Title), new CardTemplateTitleValidator());

      string error = validator.Validate<string>(nameof(CardTemplate.Title), new CardTemplate(), null);
      Assert.IsFalse(string.IsNullOrEmpty(error));
      error = validator.Validate(nameof(CardTemplate.Title), new CardTemplate(), "");
      Assert.IsFalse(string.IsNullOrEmpty(error));
      error = validator.Validate(nameof(CardTemplate.Title), new CardTemplate(), "test");
      Assert.IsTrue(string.IsNullOrEmpty(error));
    }

    /// <summary>
    /// Tests that <see cref="CardFieldDefinition.FieldName"/> is validated
    /// </summary>
    [TestMethod]
    public void ValidateFieldNamesTest()
    {
      EntityChangeValidator<CardFieldDefinition> changeValidator = new EntityChangeValidator<CardFieldDefinition>();
      changeValidator.Register(nameof(CardFieldDefinition.FieldName), new CardFieldDefinitionFieldNameValidator());
      CardTemplateChangeValidator validator = new CardTemplateChangeValidator(changeValidator);
      validator.Register(nameof(CardTemplate.Title), new CardTemplateTitleValidator());

      CardTemplate template = new CardTemplate() { Title = "test" };
      CardFieldDefinition fieldDefinition = new CardFieldDefinition();
      template.FieldDefinitions.Add(fieldDefinition);

      string error = validator.Validate<string>(nameof(CardFieldDefinition.FieldName), fieldDefinition, null);
      Assert.IsFalse(string.IsNullOrEmpty(error));
      error = validator.Validate(nameof(CardFieldDefinition.FieldName), fieldDefinition, "");
      Assert.IsFalse(string.IsNullOrEmpty(error));
      error = validator.Validate(nameof(CardFieldDefinition.FieldName), fieldDefinition, "test");
      Assert.IsTrue(string.IsNullOrEmpty(error));
    }
  }
}