using SpacedRepetitionSystem.Entities.Entities;

namespace SpacedRepetitionSystem.Entities.Validation.Core
{
  /// <summary>
  /// Class for validating the deletion of an entity
  /// </summary>
  /// <typeparam name="TEntity">Entity-Type</typeparam>
  public abstract class DeleteValidatorBase<TEntity> where TEntity : IEntity
  {
    /// <summary>
    /// Validates the deletion
    /// </summary>
    /// <param name="entity">The entity to delete</param>
    /// <returns>Error message or null</returns>
    public abstract string Validate(TEntity entity);
  }
}