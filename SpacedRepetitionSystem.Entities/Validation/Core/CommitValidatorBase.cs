using SpacedRepetitionSystem.Entities.Entities;

namespace SpacedRepetitionSystem.Entities.Validation.Core
{
  /// <summary>
  /// Validator for validating the saving of an entity
  /// </summary>
  /// <typeparam name="TEntity">Entity type</typeparam>
  public abstract class CommitValidatorBase<TEntity> where TEntity: IEntity
  {
    /// <summary>
    /// Validates the savin of the entity
    /// </summary>
    /// <param name="entity">The entity</param>
    /// <returns>Error message or null</returns>
    public abstract string Validate(TEntity entity);
  }
}