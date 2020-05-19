using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpacedRepetitionSystem.Utility.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SpacedRepetitionSystem.Utility.Tests.Extensions
{
  /// <summary>
  /// Testclass for <see cref="StringExtensions"/>
  /// </summary>
  [TestClass]
  public sealed class StringExtensionsTests
  {
    /// <summary>
    /// Tests <see cref="StringExtensions.FormatWith(string, object[])"/>
    /// </summary>
    [TestMethod]
    public void FormatWithTest()
    {
      string result = "test {0} - {1}".FormatWith(1, DateTime.Today);
      string expected = string.Format(CultureInfo.InvariantCulture, "test {0} - {1}", 1, DateTime.Today);
      Assert.AreEqual(expected, result);
    }

    /// <summary>
    /// Tests <see cref="StringExtensions.Encrypt(string)"/>
    /// </summary>
    [TestMethod]
    public void EncryptTest()
    {
      string result = "test".Encrypt();
      Assert.AreNotEqual("test", result);
      Assert.IsFalse(result.Contains("test"));
      Assert.AreNotEqual("test".Length, result.Length);
    }
  }
}