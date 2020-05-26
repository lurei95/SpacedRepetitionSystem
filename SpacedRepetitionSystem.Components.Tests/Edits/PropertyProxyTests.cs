using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpacedRepetitionSystem.Components.Edits;
using SpacedRepetitionSystem.Entities.Entities.Cards;

namespace SpacedRepetitionSystem.Components.Tests.Edits
{
  /// <summary>
  /// Testclass for <see cref="PropertyProxy"/>
  /// </summary>
  [TestClass]
  public sealed class PropertyProxyTests
  {
    /// <summary>
    /// Tests the getter and setter functionality
    /// </summary>
    [TestMethod]
    public void GetterAndSetterTest()
    {
      string setterValue = null;
      static string getter() => "get";
      void setter(string value) => setterValue = value;
      PropertyProxy propertyProxy = new PropertyProxy(getter, setter, "test", new Card());

      Assert.AreEqual("get", propertyProxy.Value);
      propertyProxy.Value = "newValue";
      Assert.AreEqual("newValue", setterValue);
    }

    /// <summary>
    /// Tests that the values set are validated
    /// </summary>
    [TestMethod]
    public void ValidatesValueTest()
    {
      bool first = true;
      Deck deck = new Deck();
      PropertyProxy propertyProxy = new PropertyProxy(() => deck.Title, (value) => deck.Title = value, nameof(Deck.Title), deck);
      propertyProxy.Validator = (value, entity) =>
      {
        if (first)
        {
          first = false;
          Assert.IsTrue(string.IsNullOrEmpty(value));
        }
        else
          Assert.AreEqual("test", value);
        Assert.AreSame(deck, entity);
        return "error";
      };
      propertyProxy.Value = "test";
      Assert.AreEqual("error", propertyProxy.ErrorText);
    }
  }
}