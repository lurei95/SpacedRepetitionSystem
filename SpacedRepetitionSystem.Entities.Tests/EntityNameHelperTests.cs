﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    [TestMethod]
    public void GetNameOfCardTest()
    {
      string name = EntityNameHelper.GetName<Card>();
      Assert.AreEqual("Card", name);
    }
    /// <summary>
    /// Tests <see cref="EntityNameHelper.GetName{Deck}"/>
    /// </summary>
    [TestMethod]
    public void GetNameOfDeckTest()
    {
      string name = EntityNameHelper.GetName<Deck>();
      Assert.AreEqual("Deck", name);
    }

    /// <summary>
    /// Tests <see cref="EntityNameHelper.GetName{CardTemplate}"/>
    /// </summary>
    [TestMethod]
    public void GetNameOfCardTemplateTest()
    {
      string name = EntityNameHelper.GetName<CardTemplate>();
      Assert.AreEqual("Template", name);
    }

    /// <summary>
    /// Tests <see cref="EntityNameHelper.GetName{User}"/>
    /// </summary>
    [TestMethod]
    public void GetNameOfUserTest()
    {
      string name = EntityNameHelper.GetName<User>();
      Assert.AreEqual("User", name);
    }

    /// <summary>
    /// Tests <see cref="EntityNameHelper.GetPluralName{Card}"/>
    /// </summary>
    [TestMethod]
    public void GetPluralNameOfCardTest()
    {
      string name = EntityNameHelper.GetPluralName<Card>();
      Assert.AreEqual("Cards", name);
    }
    /// <summary>
    /// Tests <see cref="EntityNameHelper.GetPluralName{Deck}"/>
    /// </summary>
    [TestMethod]
    public void GetPluralNameOfDeckTest()
    {
      string name = EntityNameHelper.GetPluralName<Deck>();
      Assert.AreEqual("Decks", name);
    }

    /// <summary>
    /// Tests <see cref="EntityNameHelper.GetPluralName{CardTemplate}"/>
    /// </summary>
    [TestMethod]
    public void GetPluralNameOfCardTemplateTest()
    {
      string name = EntityNameHelper.GetPluralName<CardTemplate>();
      Assert.AreEqual("Templates", name);
    }

    /// <summary>
    /// Tests <see cref="EntityNameHelper.GetPluralName{User}"/>
    /// </summary>
    [TestMethod]
    public void GetPluralNameOfUserTest()
    {
      string name = EntityNameHelper.GetPluralName<User>();
      Assert.AreEqual("Users", name);
    }
  }
}