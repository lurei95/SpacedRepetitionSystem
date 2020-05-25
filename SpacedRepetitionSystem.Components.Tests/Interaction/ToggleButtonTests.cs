using Blazorise;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpacedRepetitionSystem.Components.Interaction;

namespace SpacedRepetitionSystem.Components.Tests.Interaction
{
  /// <summary>
  /// Testclass for <see cref="ToggleButton"/>
  /// </summary>
  [TestClass]
  public sealed class ToggleButtonTests
  {
    /// <summary>
    /// Tests <see cref="ToggleButton.ToggleAction"/> is called when <see cref="ToggleButton.Value"/> changes
    /// </summary>
    [TestMethod]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "BL0005:Component parameter should not be set outside of its component.", Justification = "<Pending>")]
    public void InvokesToggleActionOnValueChanged()
    {
      Card card = new Card();
      ToggleButton button = new ToggleButton
      { ActionParameter = card };
      bool wasActionCalled = false;
      button.ToggleAction = (value, parameter) =>
      {
        Assert.IsTrue(value);
        Assert.AreSame(card, parameter);
        wasActionCalled = true;
      };
      button.Value = true;
      Assert.IsTrue(wasActionCalled);
    }
  }
}