using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Entities.Validation.Cards;

namespace SpacedRepetitionSystem.Entities.Tests.Validation.Cards
{
  /// <summary>
  /// Testclass for <see cref="CardCardTemplateIdValidator"/>
  /// </summary>
  [TestClass]
  public sealed class CardCardTemplateIdValidatorTests
  {
    /// <summary>
    /// Tests that <see cref="Card.CardTemplateId"/> is validated
    /// </summary>
    [TestMethod]
    public void ValidateTest()
    {
      CardCardTemplateIdValidator validator = new CardCardTemplateIdValidator();
      string error = validator.Validate(new Card(), default);
      Assert.IsFalse(string.IsNullOrEmpty(error));
      error = validator.Validate(new Card(), 1);
      Assert.IsTrue(string.IsNullOrEmpty(error));
    }
  }
}