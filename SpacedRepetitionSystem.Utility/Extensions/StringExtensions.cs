using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

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

    /// <summary>
    /// Encrypts a string
    /// </summary>
    /// <param name="text">the text to encrypt</param>
    /// <returns>The encrypted text</returns>
    public static string Encrypt(this string text)
    {
      MD5 provider = MD5.Create();
      string salt = "S0m3R@nd0mSalt";
      byte[] bytes = provider.ComputeHash(Encoding.UTF32.GetBytes(salt + text));
      return BitConverter.ToString(bytes).Replace("-", "").ToLower();
    }
  }
}