using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpacedRepetitionSystem.Utility.Extensions;
using System.Collections.Generic;

namespace SpacedRepetitionSystem.Utility.Tests.Extensions
{
  /// <summary>
  /// Tests for <see cref="CollectionExtensions"/>
  /// </summary>
  [TestClass]
  public sealed class CollectionExtensionsTests
  {
    /// <summary>
    /// Tests <see cref="CollectionExtensions.Shuffle{T}(IList{T})"/>
    /// </summary>
    [TestMethod]
    public void ShuffleTest()
    {
      List<int> list = new List<int>() 
      { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30 };
      list.Shuffle();
      bool isSameOrder = true;
      for (int i = 0; i < 30; i++)
        if (i + 1 != list[i])
          isSameOrder = false;
      Assert.IsFalse(isSameOrder);
    }
  }
}
