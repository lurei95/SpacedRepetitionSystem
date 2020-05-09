using Microsoft.EntityFrameworkCore;
using SpacedRepetitionSystem.Entities.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Logic.Controllers.Core
{
  /// <summary>
  /// Interface for an ApiConnector
  /// </summary>
  public interface IApiConnector
  {
    /// <summary>
    /// DbContext
    /// </summary>
    DbContext Context { get; set; }

    /// <summary>
    /// Returns the entity with the Id
    /// </summary>
    /// <param name="id">Id of the entity</param>
    /// <returns>The entity with the Id</returns>
    TEntity Get<TEntity>(object id) where TEntity : IEntity; 

    /// <summary>
    /// Returns a list of entities matching the filter
    /// </summary>
    /// <param name="searchParameters">A dictionary containing the search parameters</param>
    /// <returns>A list of entities matching the filter</returns>
    Task<List<TEntity>> Get<TEntity>(IDictionary<string, object> searchParameters) where TEntity : IEntity;

    /// <summary>
    /// Updates an existing entity
    /// </summary>
    /// <param name="entity">The updated entity</param>
    bool Put<TEntity>(TEntity entity) where TEntity : IEntity;

    /// <summary>
    /// Deletes an existing entity
    /// </summary>
    /// <param name="entity">The entity to delete</param>
    bool Delete<TEntity>(TEntity entity) where TEntity : IEntity;

    /// <summary>
    /// Creates a new entity
    /// </summary>
    /// <param name="entity">The new entity</param>
    bool Post<TEntity>(TEntity entity) where TEntity : IEntity;
  }
}