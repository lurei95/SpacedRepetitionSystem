using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Entities.Validation.CardTemplates;

namespace SpacedRepetitionSystem.Entities.Tests.Validation.CardTemplates
{
  /// <summary>
  /// Testclass for <see cref="CardFieldDefinitionFieldNameValidator"/>
  /// </summary>
  [TestClass]
  public sealed class CardFieldDefinitionFieldNameValidatorTests
  {
    /// <summary>
    /// Tests that <see cref="CardFieldDefinition.FieldName"/> is validated
    /// </summary>
    [TestMethod]
    public void ValidateTest()
    {
      CardFieldDefinitionFieldNameValidator validator = new CardFieldDefinitionFieldNameValidator();
      string error = validator.Validate(new CardFieldDefinition(), null);
      Assert.IsFalse(string.IsNullOrEmpty(error));
      error = validator.Validate(new CardFieldDefinition(), "");
      Assert.IsFalse(string.IsNullOrEmpty(error));
      error = validator.Validate(new CardFieldDefinition(), "test");
      Assert.IsTrue(string.IsNullOrEmpty(error));
    }
  }
}