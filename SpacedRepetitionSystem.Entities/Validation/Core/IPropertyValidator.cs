namespace SpacedRepetitionSystem.Entities.Validation.Core
{
  /// <summary>
  /// Interface for a property validator
  /// </summary>
  public interface IPropertyValidator
  {
    /// <summary>
    /// Name of the property
    /// </summary>
    string PropertyName { get; }
  }
}