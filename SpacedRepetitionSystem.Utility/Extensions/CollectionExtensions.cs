using System;
using System.Collections.Generic;

namespace SpacedRepetitionSystem.Utility.Extensions
{
  /// <summary>
  /// Contains extension methods for <see cref="ICollection{T}"/>
  /// </summary>
  public static class CollectionExtensions
  {
    private static readonly Random random = new Random();

    /// <summary>
    /// Shuffles a list
    /// </summary>
    /// <typeparam name="T">Generic parameter</typeparam>
    /// <param name="list">The list to shuffle</param>
    public static void Shuffle<T>(this IList<T> list)
    {
      int n = list.Count;
      while (n > 1)
      {
        n--;
        int k = random.Next(n + 1);
        T value = list[k];
        list[k] = list[n];
        list[n] = value;
      }
    }
  }
}
