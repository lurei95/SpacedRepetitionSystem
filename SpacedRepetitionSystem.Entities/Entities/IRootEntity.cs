namespace SpacedRepetitionSystem.Entities.Entities
{
  /// <summary>
  /// Interface for a root entity
  /// </summary>
  public interface IRootEntity : IEntity
  {
    /// <summary>
    /// Route for the entity
    /// </summary>
    string Route { get; }
  }
}