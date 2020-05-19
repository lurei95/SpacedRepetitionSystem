using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.WebAPI.Validation.CardTemplates;

namespace SpacedRepetitionSystem.WebApi.Tests.Validation.CardTemplates
{
  /// <summary>
  /// Testclass for <see cref="CardTemplateDeleteValidatorTests"/>
  /// </summary>
  [TestClass]
  public sealed class CardTemplateDeleteValidatorTests : EntityFrameWorkTestBase
  {
    private static readonly CardTemplate template = new CardTemplate()
    {
      CardTemplateId = 1,
      Title = "test"
    };

    /// <summary>
    /// Tests that a template cannot be deleted if a card with the template still exists
    /// </summary>
    [TestMethod]
    public void CannotDeleteTemplateIfCardWithTemplateExists()
    {
      CreateData(context =>
      {
        Card card = new Card()
        {
          CardTemplateId = 1,
          DeckId = 1
        };
        context.Add(template);
        context.Add(card);
      });

      using DbContext context = CreateContext();
      CardTemplateDeleteValidator validator = new CardTemplateDeleteValidator(context);
      string error = validator.Validate(template);
      Assert.IsFalse(string.IsNullOrEmpty(error));

      Card card = context.Find<Card>((long)1);
      context.Remove(card);
      context.SaveChanges();
      error = validator.Validate(template);
      Assert.IsTrue(string.IsNullOrEmpty(error));
    }

    /// <summary>
    /// Tests that a template cannot be deleted if a deck with the template still exists
    /// </summary>
    [TestMethod]
    public void CannotDeleteTemplateIfDeckWithTemplateExists()
    {
      CreateData(context =>
      {
        Deck deck = new Deck()
        {
          Title = "test",
          DefaultCardTemplateId = 1,
          DeckId = 1
        };
        context.Add(template);
        context.Add(deck);
      });

      using DbContext context = CreateContext();
      CardTemplateDeleteValidator validator = new CardTemplateDeleteValidator(context);
      string error = validator.Validate(template);
      Assert.IsFalse(string.IsNullOrEmpty(error));

      Deck deck = context.Find<Deck>((long)1);
      context.Remove(deck);
      context.SaveChanges();
      error = validator.Validate(template);
      Assert.IsTrue(string.IsNullOrEmpty(error));
    }
  }
}