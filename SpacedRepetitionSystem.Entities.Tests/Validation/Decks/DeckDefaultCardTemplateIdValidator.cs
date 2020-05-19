using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Entities.Validation.Decks;

namespace SpacedRepetitionSystem.Entities.Tests.Validation.Cards
{
  /// <summary>
  /// Testclass for <see cref="DeckDefaultCardTemplateIdValidator"/>
  /// </summary>
  [TestClass]
  public sealed class DeckDefaultCardTemplateIdValidatorTests
  {
    /// <summary>
    /// Tests that <see cref="Deck.DefaultCardTemplateId"/> is validated
    /// </summary>
    [TestMethod]
    public void ValidateTest()
    {
      DeckDefaultCardTemplateIdValidator validator = new DeckDefaultCardTemplateIdValidator();
      string error = validator.Validate(new Deck(), default);
      Assert.IsFalse(string.IsNullOrEmpty(error));
      error = validator.Validate(new Deck(), 1);
      Assert.IsTrue(string.IsNullOrEmpty(error));
    }
  }
}