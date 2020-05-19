using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Entities.Validation.Decks;

namespace SpacedRepetitionSystem.Entities.Tests.Validation.CardTemplates
{
  /// <summary>
  /// Testclass for <see cref="Deck.Title"/>
  /// </summary>
  [TestClass]
  public sealed class DeckTitleValidatorTests
  {
    /// <summary>
    /// Tests that <see cref="Deck.Title"/> is validated
    /// </summary>
    [TestMethod]
    public void ValidateTest()
    {
      DeckTitleValidator validator = new DeckTitleValidator();
      string error = validator.Validate(new Deck(), null);
      Assert.IsFalse(string.IsNullOrEmpty(error));
      error = validator.Validate(new Deck(), "");
      Assert.IsFalse(string.IsNullOrEmpty(error));
      error = validator.Validate(new Deck(), "test");
      Assert.IsTrue(string.IsNullOrEmpty(error));
    }
  }
}