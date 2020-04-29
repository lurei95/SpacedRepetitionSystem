using Microsoft.EntityFrameworkCore;
using SpacedRepetitionSystem.Entities.Entities;

namespace SpacedRepetitionSystem.Entities.Validation.Core
{
  /// <summary>
  /// Class for validating the deletion of an entity
  /// </summary>
  /// <typeparam name="TEntity">Entity-Type</typeparam>
  public class DeleteValidatorBase<TEntity> where TEntity : IEntity
  {
    /// <summary>
    /// Context
    /// </summary>
    protected DbContext Context { get; private set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="context">Context (injected)</param>
    public DeleteValidatorBase(DbContext context) => Context = context;

    /// <summary>
    /// Validates the deletion
    /// </summary>
    /// <param name="entity">The entity to delete</param>
    /// <returns>Error message or null</returns>
    public virtual string Validate(TEntity entity) => null;
  }
}