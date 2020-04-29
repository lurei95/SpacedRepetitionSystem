using Microsoft.EntityFrameworkCore;
using SpacedRepetitionSystem.Entities.Entities;
using SpacedRepetitionSystem.Entities.Validation.Core;
using SpacedRepetitionSystem.Utility.Notification;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Logic.Controllers.Core
{
  /// <summary>
  /// Base class for a controller for a specific entity
  /// </summary>
  /// <typeparam name="TEntity"></typeparam>
  public abstract class EntityControllerBase<TEntity> where TEntity : IEntity
  {
    private readonly DeleteValidatorBase<TEntity> deleteValidator;
    private readonly CommitValidatorBase<TEntity> commitValidator;

    /// <summary>
    /// Context used to perform the actions
    /// </summary>
    protected DbContext Context { get; private set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="context">Context (injected)</param>
    /// <param name="commitValidator">CommitValidator (injected)</param>
    /// <param name="deleteValidator">DeleteValidator (injected)</param>
    public EntityControllerBase(DbContext context, DeleteValidatorBase<TEntity> deleteValidator, CommitValidatorBase<TEntity> commitValidator) 
    { 
      Context = context;
      this.deleteValidator = deleteValidator;
      this.commitValidator = commitValidator;
    }

    /// <summary>
    /// Returns the entity with the Id
    /// </summary>
    /// <param name="id">Id of the entity</param>
    /// <returns>The entity with the Id</returns>
    public abstract TEntity Get(object id);

    /// <summary>
    /// Returns a list of entities matching the filter
    /// </summary>
    /// <param name="searchParameters">A dictionary containing the search parameters</param>
    /// <returns>A list of entities matching the filter</returns>
    public abstract Task<List<TEntity>> Get(IDictionary<string, object> searchParameters);

    /// <summary>
    /// Updates an existing entity
    /// </summary>
    /// <param name="entity">The updated entity</param>
    public void Put(TEntity entity)
    {
      string error = commitValidator.Validate(entity);
      if (string.IsNullOrEmpty(error))
      {
        UnitOfWork unitOfWork = new UnitOfWork(Context);
        unitOfWork.Execute(() => PutCore(entity));
      }
      else
        throw new NotifyException(error);
    }

    /// <summary>
    /// Creates a new entity
    /// </summary>
    /// <param name="entity">The new entity</param>
    public void Post(TEntity entity)
    {
      string error = commitValidator.Validate(entity);
      if (string.IsNullOrEmpty(error))
      {
        UnitOfWork unitOfWork = new UnitOfWork(Context);
        unitOfWork.Execute(() => PostCore(entity));
      }
      else
        throw new NotifyException(error);
    }

    /// <summary>
    /// Deletes an existing entity
    /// </summary>
    /// <param name="entity">The entity to delete</param>
    public void Delete(TEntity entity)
    {
      string error = deleteValidator.Validate(entity);
      if (string.IsNullOrEmpty(error))
      {
        UnitOfWork unitOfWork = new UnitOfWork(Context);
        unitOfWork.Execute(() => DeleteCore(entity));
      }
      else
        throw new NotifyException(error);
    }

    /// <summary>
    /// Updates an existing entity
    /// </summary>
    /// <param name="entity">The updated entity</param>
    protected virtual void PutCore(TEntity entity) { }

    /// <summary>
    /// Deletes an existing entity
    /// </summary>
    /// <param name="entity">The entity to delete</param>
    protected virtual void DeleteCore(TEntity entity) => Context.Remove(entity);

    /// <summary>
    /// Creates a new entity
    /// </summary>
    /// <param name="entity">The new entity</param>
    protected virtual void PostCore(TEntity entity) => Context.Add(entity);
  }
}