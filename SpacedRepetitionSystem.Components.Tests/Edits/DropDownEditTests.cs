using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpacedRepetitionSystem.Components.Edits;
using System.Collections.Generic;

namespace SpacedRepetitionSystem.Components.Tests.Edits
{
  /// <summary>
  /// Testclass for <see cref="TestEdit"/>
  /// </summary>
  [TestClass]
  public sealed class DropDownEditTests
  {
    /// <summary>
    /// Tests that the text changed event is invoked when the value is changed
    /// </summary>
    [TestMethod]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "BL0005:Component parameter should not be set outside of its component.", Justification = "<Pending>")]
    public void SetsValueWhenSelectableValuesIsSetTests()
    {
      DropDownEdit edit = new DropDownEdit()
      { SelectableValues = new List<string>() { "test", "test1" }  };
      Assert.AreEqual("test", edit.Value);
    }
  }
}