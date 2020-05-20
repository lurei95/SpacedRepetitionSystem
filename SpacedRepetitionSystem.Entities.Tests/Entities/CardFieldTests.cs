using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using System;

namespace SpacedRepetitionSystem.Entities.Tests.Entities
{
  /// <summary>
  /// Testclass for <see cref="CardField"/>
  /// </summary>
  [TestClass]
  public sealed class CardFieldTests
  {
    /// <summary>
    /// Tests <see cref="CardField.IsDue"/>
    /// </summary>
    [TestMethod]
    public void IsDueTest()
    {
      CardField field = new CardField { DueDate = DateTime.Today.AddDays(1) };
      Assert.IsFalse(field.IsDue);
      field.DueDate = DateTime.Today;
      Assert.IsTrue(field.IsDue);
    }
  }
}