using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Entities.Entities.Security;

namespace SpacedRepetitionSystem.Entities.Tests
{
  /// <summary>
  /// Testclass for <see cref="EntityNameHelper"/>
  /// </summary>
  [TestClass]
  public sealed class EntityNameHelperTests
  {
    /// <summary>
    /// Tests <see cref="EntityNameHelper.GetName{Card}"/>
    /// </summary>
    public void GetNameOfCardTest()
    {
      string name = EntityNameHelper.GetName<Card>();
      Assert.AreEqual("Card", name);
    }
    /// <summary>
    /// Tests <see cref="EntityNameHelper.GetName{Deck}"/>
    /// </summary>
    public void GetNameOfDeckTest()
    {
      string name = EntityNameHelper.GetName<Deck>();
      Assert.AreEqual("Deck", name);
    }

    /// <summary>
    /// Tests <see cref="EntityNameHelper.GetName{CardTemplate}"/>
    /// </summary>
    public void GetNameOfCardTemplateTest()
    {
      string name = EntityNameHelper.GetName<CardTemplate>();
      Assert.AreEqual("Template", name);
    }

    /// <summary>
    /// Tests <see cref="EntityNameHelper.GetName{User}"/>
    /// </summary>
    public void GetNameOfUserTest()
    {
      string name = EntityNameHelper.GetName<User>();
      Assert.AreEqual("User", name);
    }
  }
}
