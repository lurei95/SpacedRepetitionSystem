using System.Collections.Generic;

namespace SpacedRepetitionSystem.Components.ViewModels.Cards
{
  /// <summary>
  /// Practice Result
  /// </summary>
  public class PracticeResult
  {
    /// <summary>
    /// Correct count
    /// </summary>
    public int Correct { get; set; }

    /// <summary>
    /// Difficult count
    /// </summary>
    public int Difficult { get; set; }

    /// <summary>
    /// Wrong count
    /// </summary>
    public int Wrong { get; set; }
  }

  /// <summary>
  /// Practice result for a card
  /// </summary>
  public sealed class CardPracticeResult : PracticeResult
  {
    /// <summary>
    /// Pratcice results of the fields of the card
    /// </summary>
    public Dictionary<string, PracticeResult> FieldResults { get; } = new Dictionary<string, PracticeResult>(); 
  }
}
