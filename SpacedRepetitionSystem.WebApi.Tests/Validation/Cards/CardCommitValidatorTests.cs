using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.WebAPI.Validation.Cards;

namespace SpacedRepetitionSystem.WebApi.Tests.Validation.Cards
{
  /// <summary>
  /// Testclass for <see cref="CardCommitValidator"/>
  /// </summary>
  [TestClass]
  public sealed class CardCommitValidatorTests : EntityFrameWorkTestBase
  {
    /// <summary>
    /// Tests the validation of <see cref="Card.CardTemplateId"/>
    /// </summary>
    [TestMethod]
    public void ValidatesCardTemplateIdTest()
    {
      using DbContext context = CreateContext();
      CardCommitValidator validator = new CardCommitValidator(context);
      Card card = new Card()
      {
        CardId = 1,
        DeckId = 1
      };
      card.Fields.Add(new CardField()
      {
        CardId = 1,
        FieldName = "TestField",
        Value = "test"
      });

      //not successful
      string error = validator.Validate(card);
      Assert.IsFalse(string.IsNullOrEmpty(error));

      //successful
      card.CardTemplateId = 1;
      error = validator.Validate(card);
      Assert.IsTrue(string.IsNullOrEmpty(error));
    }

    /// <summary>
    /// Tests the validation of <see cref="Card.DeckId"/>
    /// </summary>
    [TestMethod]
    public void ValidatesDeckIdTest()
    {
      using DbContext context = CreateContext();
      CardCommitValidator validator = new CardCommitValidator(context);
      Card card = new Card()
      {
        CardTemplateId = 1,
        CardId = 1
      };
      card.Fields.Add(new CardField()
      {
        CardId = 1,
        FieldName = "TestField",
        Value = "test"
      });

      //not successful
      string error = validator.Validate(card);
      Assert.IsFalse(string.IsNullOrEmpty(error));

      //successful
      card.DeckId = 1;
      error = validator.Validate(card);
      Assert.IsTrue(string.IsNullOrEmpty(error));
    }

    /// <summary>
    /// Tests <see cref="Car"/>
    /// </summary>
    [TestMethod]
    public void ValidatesHasFieldWithValueTest()
    {
      using DbContext context = CreateContext();
      CardCommitValidator validator = new CardCommitValidator(context);
      Card card = new Card()
      {
        CardTemplateId = 1,
        CardId = 1,
        DeckId = 1
      };
      card.Fields.Add(new CardField()
      {
        CardId = 1,
        FieldName = "TestField1",
      });
      card.Fields.Add(new CardField()
      {
        CardId = 1,
        FieldName = "TestField2",
      });
      //not successful
      string error = validator.Validate(card);
      Assert.IsFalse(string.IsNullOrEmpty(error));

      //successful
      card.Fields[0].Value = "test";
      error = validator.Validate(card);
      Assert.IsTrue(string.IsNullOrEmpty(error));
    }
  }
}