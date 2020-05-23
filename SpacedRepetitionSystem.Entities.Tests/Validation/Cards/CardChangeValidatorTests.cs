using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Entities.Validation.Cards;
using SpacedRepetitionSystem.Entities.Validation.Core;

namespace SpacedRepetitionSystem.Entities.Tests.Validation.Cards
{
  /// <summary>
  /// Testclass for <see cref="CardChangeValidator"/>
  /// </summary>
  [TestClass]
  public sealed class CardChangeValidatorTests
  {
    /// <summary>
    /// Tests that <see cref="Card.DeckId"/> is validated
    /// </summary>
    [TestMethod]
    public void ValidateDeckIdTest()
    {
      EntityChangeValidator<CardField> changeValidator = new EntityChangeValidator<CardField>();
      CardChangeValidator validator = new CardChangeValidator(changeValidator);
      validator.Register(nameof(Card.DeckId), new CardDeckIdValidator());

      string error = validator.Validate<long>(nameof(Card.DeckId), new Card(), default);
      Assert.IsFalse(string.IsNullOrEmpty(error));
      error = validator.Validate(nameof(Card.DeckId), new Card(), (long)1);
      Assert.IsTrue(string.IsNullOrEmpty(error));
    }

    /// <summary>
    /// Tests that <see cref="Card.CardTemplateId"/> is validated
    /// </summary>
    [TestMethod]
    public void ValidateCardTemplateIdTest()
    {
      EntityChangeValidator<CardField> changeValidator = new EntityChangeValidator<CardField>();
      CardChangeValidator validator = new CardChangeValidator(changeValidator);
      validator.Register(nameof(Card.DeckId), new CardCardTemplateIdValidator());

      string error = validator.Validate<long>(nameof(Card.DeckId), new Card(), default);
      Assert.IsFalse(string.IsNullOrEmpty(error));
      error = validator.Validate(nameof(Card.DeckId), new Card(), (long)1);
      Assert.IsTrue(string.IsNullOrEmpty(error));
    }

    /// <summary>
    /// Tests that <see cref="CardField.Value"/> is validated
    /// </summary>
    [TestMethod]
    public void ValidateFieldNamesTest()
    {
      EntityChangeValidator<CardField> changeValidator = new EntityChangeValidator<CardField>();
      changeValidator.Register(nameof(CardField.Value), new CardFieldValueValidator());
      CardChangeValidator validator = new CardChangeValidator(changeValidator);
      CardField field = new CardField
      { CardFieldDefinition = new CardFieldDefinition() { IsRequired = false } };

      string error = validator.Validate<string>(nameof(CardField.Value), field, null);
      Assert.IsTrue(string.IsNullOrEmpty(error));
      error = validator.Validate(nameof(CardField.Value), field, "");
      Assert.IsTrue(string.IsNullOrEmpty(error));

      field.CardFieldDefinition.IsRequired = true;
      error = validator.Validate<string>(nameof(CardField.Value), field, null);
      Assert.IsFalse(string.IsNullOrEmpty(error));
      error = validator.Validate(nameof(CardField.Value), field, "");
      Assert.IsFalse(string.IsNullOrEmpty(error));
      error = validator.Validate(nameof(CardField.Value), field, "test");
      Assert.IsTrue(string.IsNullOrEmpty(error));
    }
  }
}