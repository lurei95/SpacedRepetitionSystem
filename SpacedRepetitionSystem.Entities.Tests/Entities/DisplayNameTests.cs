using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Entities.Entities.Security;

namespace SpacedRepetitionSystem.Entities.Tests.Entities
{
  /// <summary>
  /// Testclass for testing the display name of entities
  /// </summary>
  [TestClass]
  public sealed class DisplayNameTests
  {
    /// <summary>
    /// Tests <see cref="Card.GetDisplayName"/>
    /// </summary>
    [TestMethod]
    public void CardDisplayNameTest()
    {
      string displayName = new Card() { CardId = 2 }.GetDisplayName();
      Assert.AreEqual("Card - 2", displayName);
    }

    /// <summary>
    /// Tests <see cref="Deck.GetDisplayName"/>
    /// </summary>
    [TestMethod]
    public void DeckDisplayNameTest()
    {
      string displayName = new Deck() { Title = "test" }.GetDisplayName();
      Assert.AreEqual("Deck - test", displayName);
    }

    /// <summary>
    /// Tests <see cref="CardTemplate.GetDisplayName"/>
    /// </summary>
    [TestMethod]
    public void CardTemplateDisplayNameTest()
    {
      string displayName = new CardTemplate() { Title = "test" }.GetDisplayName();
      Assert.AreEqual("Template - test", displayName);
    }

    /// <summary>
    /// Tests <see cref="User.GetDisplayName"/>
    /// </summary>
    [TestMethod]
    public void UserDisplayNameTest()
    {
      string displayName = new User() { Email = "test@test" }.GetDisplayName();
      Assert.AreEqual("User - test@test", displayName);
    }
  }
}