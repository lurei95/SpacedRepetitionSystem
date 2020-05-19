using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Entities.Validation.Cards;

namespace SpacedRepetitionSystem.Entities.Tests.Validation.Cards
{
  /// <summary>
  /// Testclass for <see cref="CardDeckIdValidator"/>
  /// </summary>
  [TestClass]
  public sealed class CardDeckIdValdiatorTests
  {
    /// <summary>
    /// Tests that <see cref="Card.DeckId"/> is validated
    /// </summary>
    [TestMethod]
    public void ValidateTest()
    {
      CardDeckIdValidator validator = new CardDeckIdValidator();
      string error = validator.Validate(new Card(), default);
      Assert.IsFalse(string.IsNullOrEmpty(error));
      error = validator.Validate(new Card(), 1);
      Assert.IsTrue(string.IsNullOrEmpty(error));
    }
  }
}