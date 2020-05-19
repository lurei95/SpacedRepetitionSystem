using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Entities.Validation.Core;

namespace SpacedRepetitionSystem.Entities.Tests.Validation.Core
{
  /// <summary>
  /// TestClass for
  /// </summary>
  [TestClass]
  public sealed class EntityChangeValidatorTests
  {
    [TestMethod]
    public void DoesCallPropertyValidatorTest()
    {
      TestPropertyValidator propertyValidator = new TestPropertyValidator();
      EntityChangeValidator<Card> changeValidator = new EntityChangeValidator<Card>();
      changeValidator.Register(nameof(Card.CardId), propertyValidator);
      Card card = new Card();

      string result = changeValidator.Validate(nameof(Card.CardId), card, (long)4);
      Assert.AreEqual("test", result);
      Assert.AreEqual(4, propertyValidator.NewValue);
      Assert.AreSame(card, propertyValidator.Card);

      result = changeValidator.Validate(nameof(Card.CardTemplateId), card, (long)4);
      Assert.IsTrue(string.IsNullOrEmpty(result));
    }
  }

  public sealed class TestPropertyValidator : PropertyValidatorBase<Card, long>
  {
    /// <summary>
    /// New Value
    /// </summary>
    public long NewValue { get; set; }

    /// <summary>
    /// Card
    /// </summary>
    public Card Card { get; set; }

    ///<inheritdoc/>
    public override string PropertyName => nameof(Card.CardId);

    ///<inheritdoc/>
    public override string Validate(Card entity, long newValue)
    {
      NewValue = newValue;
      Card = entity;
      return "test";
    }
  }
}