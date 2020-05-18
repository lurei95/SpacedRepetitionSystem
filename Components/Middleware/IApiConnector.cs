using SpacedRepetitionSystem.Entities.Entities;
using SpacedRepetitionSystem.Entities.Entities.Security;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Components.Middleware
{
  /// <summary>
  /// Interface for an ApiConnector
  /// </summary>
  public interface IApiConnector
  {
    /// <summary>
    /// The current user
    /// </summary>
    public User CurrentUser { get; set; }

    /// <summary>
    /// Returns the entity with the Id
    /// </summary>
    /// <param name="id">Id of the entity</param>
    /// <returns>The entity with the Id</returns>
    Task<ApiReply<TEntity>> GetAsync<TEntity>(object id) where TEntity : IRootEntity, new(); 

    /// <summary>
    /// Returns a list of entities matching the filter
    /// </summary>
    /// <param name="searchParameters">A dictionary containing the search parameters</param>
    /// <returns>A list of entities matching the filter</returns>
    Task<ApiReply<List<TEntity>>> GetAsync<TEntity>(IDictionary<string, object> searchParameters) where TEntity : IRootEntity, new();

    /// <summary>
    /// Updates an existing entity
    /// </summary>
    /// <param name="entity">The updated entity</param>
    Task<ApiReply> PutAsync<TEntity>(TEntity entity) where TEntity : IRootEntity;

    /// <summary>
    /// Deletes an existing entity
    /// </summary>
    /// <param name="entity">The entity to delete</param>
    Task<ApiReply> DeleteAsync<TEntity>(TEntity entity) where TEntity : IRootEntity;

    /// <summary>
    /// Creates a new entity
    /// </summary>
    /// <param name="entity">The new entity</param>
    Task<ApiReply> PostAsync<TEntity>(TEntity entity) where TEntity : IRootEntity;

    /// <summary>
    /// Performs a post request to the specified route with value as content
    /// </summary>
    /// <param name="route">The route to post to</param>
    /// <param name="value">The value that is sent in the request body</param>
    /// <typeparam name="TReturn">Type of the return value</typeparam>
    Task<ApiReply<TReturn>> PostAsync<TReturn>(string route, object value);

    /// <summary>
    /// Performs a post request to the specified route with value as content
    /// </summary>
    /// <param name="route">The route to post to</param>
    /// <param name="value">The value to be sent</param>
    Task<ApiReply> PostAsync(string route, object value);
  }
}