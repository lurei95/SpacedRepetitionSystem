using System;
using System.Globalization;

namespace SpacedRepetitionSystem.Utility.Extensions
{
  /// <summary>
  /// Class containing exrtension methods for string
  /// </summary>
  public static class StringExtensions
  {
    /// <summary>
    /// Returns a formated string
    /// </summary>
    /// <param name="text">The string to format</param>
    /// <param name="parameters">The parameters with which the string should be formated</param>
    /// <returns>The formated string</returns>
    public static string FormatWith(this string text, params object[] parameters)
      => String.Format(CultureInfo.InvariantCulture, text, parameters);
  }
}