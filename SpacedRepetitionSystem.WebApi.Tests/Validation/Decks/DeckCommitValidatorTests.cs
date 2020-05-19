using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Entities.Entities.Security;
using SpacedRepetitionSystem.WebAPI.Validation.Decks;
using System;

namespace SpacedRepetitionSystem.WebApi.Tests.Validation.Cards
{
  /// <summary>
  /// Testclass for <see cref="DeckCommitValidator"/>
  /// </summary>
  [TestClass]
  public sealed class DeckCommitValidatorTests : EntityFrameWorkTestBase
  {
    /// <summary>
    /// Tests the validation of <see cref="Deck.DefaultCardTemplateId"/>
    /// </summary>
    [TestMethod]
    public void ValidatesDefaultCardTemplateIdTest()
    {
      using DbContext context = CreateContext();
      DeckCommitValidator validator = new DeckCommitValidator(context);
      Deck deck = new Deck()
      {
        Title = "test",
        DeckId = 1
      };

      //not successful
      string error = validator.Validate(deck);
      Assert.IsFalse(string.IsNullOrEmpty(error));

      //successful
      deck.DefaultCardTemplateId = 1;
      error = validator.Validate(deck);
      Assert.IsTrue(string.IsNullOrEmpty(error));
    }

    /// <summary>
    /// Tests the validation of <see cref="Deck.Title"/>
    /// </summary>
    [TestMethod]
    public void ValidatesTitleTest()
    {
      using DbContext context = CreateContext();
      DeckCommitValidator validator = new DeckCommitValidator(context);
      Deck deck = new Deck()
      {
        DefaultCardTemplateId = 1,
        DeckId = 1
      };

      //not successful
      string error = validator.Validate(deck);
      Assert.IsFalse(string.IsNullOrEmpty(error));

      //successful
      deck.Title = "Test";
      error = validator.Validate(deck);
      Assert.IsTrue(string.IsNullOrEmpty(error));
    }

    /// <summary>
    /// Tests that the title of the deck is unique
    /// </summary>
    [TestMethod]
    public void ValidatesTitleUniqueTest()
    {
      Guid userId = Guid.NewGuid();
      CreateData((context) =>
      {
        User user = new User() { UserId = userId };
        Deck deck1 = new Deck()
        {
          UserId = userId,
          Title = "test1",
          DefaultCardTemplateId = 1,
          DeckId = 1
        };
        Deck deck2 = new Deck()
        {
          UserId = Guid.NewGuid(),
          Title = "test2",
          DefaultCardTemplateId = 1,
          DeckId = 2
        };
        context.Add(user);
        context.Add(deck1);
        context.Add(deck2);
      });

      //not successful
      using DbContext context = CreateContext();
      DeckCommitValidator validator = new DeckCommitValidator(context);
      Deck deck = new Deck()
      {
        UserId = userId,
        Title = "test1",
        DefaultCardTemplateId = 1,
        DeckId = 3
      };
      string error = validator.Validate(deck);
      Assert.IsFalse(string.IsNullOrEmpty(error));

      //successful
      deck = new Deck()
      {
        Title = "test2",
        DefaultCardTemplateId = 1,
        DeckId = 3
      };
      error = validator.Validate(deck);
      Assert.IsTrue(string.IsNullOrEmpty(error));
    }
  }
}