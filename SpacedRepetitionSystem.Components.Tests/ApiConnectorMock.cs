using SpacedRepetitionSystem.Components.Middleware;
using SpacedRepetitionSystem.Entities.Entities;
using SpacedRepetitionSystem.Entities.Entities.Security;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Components.Tests
{
  /// <summary>
  /// Mock implementation for <see cref="IApiConnector"/>
  /// </summary>
  public sealed class ApiConnectorMock : IApiConnector
  {
    /// <summary>
    /// The CurrentUser
    /// </summary>
    public User CurrentUser { get; set; }

    /// <summary>
    /// The parameter
    /// </summary>
    public object Parameter { get; set; }

    /// <summary>
    /// The route
    /// </summary>
    public string Route { get; set; }

    /// <summary>
    /// The reply
    /// </summary>
    public Stack<ApiReply> Replies { get; } = new Stack<ApiReply>();

    /// <summary>
    /// Method
    /// </summary>
    public HttpMethod Method { get; set; }

    ///<inheritdoc/>
    public async Task<ApiReply> DeleteAsync<TEntity>(TEntity entity) where TEntity : IRootEntity
    {
      Parameter = entity;
      Method = HttpMethod.Delete;
      return await Task.FromResult(Replies.Pop());
    }

    ///<inheritdoc/>
    public async Task<ApiReply<TEntity>> GetAsync<TEntity>(object id) where TEntity : IRootEntity, new()
    {
      Parameter = id;
      Method = HttpMethod.Get;
      return await Task.FromResult(Replies.Pop() as ApiReply<TEntity>);
    }

    ///<inheritdoc/>
    public async Task<ApiReply<List<TEntity>>> GetAsync<TEntity>(IDictionary<string, object> searchParameters) where TEntity : IRootEntity, new()
    {
      Parameter = searchParameters;
      Method = HttpMethod.Get;
      return await Task.FromResult(Replies.Pop() as ApiReply<List<TEntity>>);
    }

    ///<inheritdoc/>
    public async Task<ApiReply> PostAsync<TEntity>(TEntity entity) where TEntity : IRootEntity
    {
      Parameter = entity;
      Method = HttpMethod.Post;
      return await Task.FromResult(Replies.Pop());
    }

    ///<inheritdoc/>
    public async Task<ApiReply<TReturn>> PostAsync<TReturn>(string route, object value)
    {
      Route = route;
      Parameter = value;
      Method = HttpMethod.Post;
      return await Task.FromResult(Replies.Pop() as ApiReply<TReturn>);
    }

    ///<inheritdoc/>
    public async Task<ApiReply> PostAsync(string route, object value)
    {
      Route = route;
      Parameter = value;
      Method = HttpMethod.Post;
      return await Task.FromResult(Replies.Pop());
    }

    ///<inheritdoc/>
    public async Task<ApiReply> PutAsync<TEntity>(TEntity entity) where TEntity : IRootEntity
    {
      Parameter = entity;
      Method = HttpMethod.Put;
      return await Task.FromResult(Replies.Pop() as ApiReply);
    }
  }
}