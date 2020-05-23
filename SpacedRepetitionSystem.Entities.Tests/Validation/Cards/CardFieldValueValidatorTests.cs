using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Entities.Validation.Cards;

namespace SpacedRepetitionSystem.Entities.Tests.Validation.Cards
{
  /// <summary>
  /// Testclass for <see cref="CardFieldValueValidator"/>
  /// </summary>
  [TestClass]
  public sealed class CardFieldValueValidatorTests
  {
    /// <summary>
    /// Tests that <see cref="CardField.Value"/> is validated
    /// </summary>
    [TestMethod]
    public void ValidateTest()
    {
      CardFieldValueValidator validator = new CardFieldValueValidator();
      CardField field = new CardField
      { CardFieldDefinition = new CardFieldDefinition() { IsRequired = false } };

      string error = validator.Validate(field, null);
      Assert.IsTrue(string.IsNullOrEmpty(error));
      error = validator.Validate(field, "");
      Assert.IsTrue(string.IsNullOrEmpty(error));

      field.CardFieldDefinition.IsRequired = true;
      error = validator.Validate(field, null);
      Assert.IsFalse(string.IsNullOrEmpty(error));
      error = validator.Validate(field, "");
      Assert.IsFalse(string.IsNullOrEmpty(error));
      error = validator.Validate(field, "test");
      Assert.IsTrue(string.IsNullOrEmpty(error));
    }
  }
}