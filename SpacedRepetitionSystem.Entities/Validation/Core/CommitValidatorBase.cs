using Microsoft.EntityFrameworkCore;
using SpacedRepetitionSystem.Entities.Entities;

namespace SpacedRepetitionSystem.Entities.Validation.Core
{
  /// <summary>
  /// Validator for validating the saving of an entity
  /// </summary>
  /// <typeparam name="TEntity">Entity type</typeparam>
  public class CommitValidatorBase<TEntity> where TEntity: IEntity
  {
    /// <summary>
    /// Context
    /// </summary>
    protected DbContext Context { get; private set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="context">Context (injected)</param>
    public CommitValidatorBase(DbContext context) => Context = context;

    /// <summary>
    /// Validates the saving of the entity
    /// </summary>
    /// <param name="entity">The entity</param>
    /// <returns>Error message or null</returns>
    public virtual string Validate(TEntity entity) => null;
  }
}