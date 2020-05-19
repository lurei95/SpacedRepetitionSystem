using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Entities.Validation.CardTemplates;

namespace SpacedRepetitionSystem.Entities.Tests.Validation.CardTemplates
{
  /// <summary>
  /// Testclass for <see cref="CardTemplate.Title"/>
  /// </summary>
  [TestClass]
  public sealed class CardTemplateTitleValidatorTests
  {
    /// <summary>
    /// Tests that <see cref="CardTemplate.Title"/> is validated
    /// </summary>
    [TestMethod]
    public void ValidateTest()
    {
      CardTemplateTitleValidator validator = new CardTemplateTitleValidator();
      string error = validator.Validate(new CardTemplate(), null);
      Assert.IsFalse(string.IsNullOrEmpty(error));
      error = validator.Validate(new CardTemplate(), "");
      Assert.IsFalse(string.IsNullOrEmpty(error));
      error = validator.Validate(new CardTemplate(), "test");
      Assert.IsTrue(string.IsNullOrEmpty(error));
    }
  }
}