using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Logic.Controllers.Core
{
  /// <summary>
  /// Base class for a controller for a specific entity
  /// </summary>
  /// <typeparam name="TEntity"></typeparam>
  public abstract class EntityControllerBase<TEntity>
  {
    /// <summary>
    /// Context used to perform the actions
    /// </summary>
    protected DbContext Context { get; private set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="context">Context used to perform the actions</param>
    public EntityControllerBase(DbContext context) => Context = context;

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
    public abstract void Put(TEntity entity);

    /// <summary>
    /// Deletes an existing entity
    /// </summary>
    /// <param name="entity">The entity to delete</param>
    public abstract void Delete(TEntity entity);

    /// <summary>
    /// Creates a new entity
    /// </summary>
    /// <param name="entity">The new entity</param>
    public abstract void Post(TEntity entity);

    protected void ShowSaveMessage()
    {

    }
  }
}