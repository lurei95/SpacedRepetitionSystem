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
    public Stack<object> Parameters { get; } = new Stack<object>();

    /// <summary>
    /// The route
    /// </summary>
    public Stack<string> Routes { get; } = new Stack<string>();

    /// <summary>
    /// The reply
    /// </summary>
    public Stack<ApiReply> Replies { get; } = new Stack<ApiReply>();

    /// <summary>
    /// Method
    /// </summary>
    public Stack<HttpMethod> Methods { get; } = new Stack<HttpMethod>();

    ///<inheritdoc/>
    public async Task<ApiReply> DeleteAsync<TEntity>(TEntity entity) where TEntity : IRootEntity
    {
      Parameters.Push(entity);
      Methods.Push(HttpMethod.Delete);
      return await Task.FromResult(Replies.Pop());
    }

    ///<inheritdoc/>
    public async Task<ApiReply<TEntity>> GetAsync<TEntity>(object id) where TEntity : IRootEntity, new()
    {
      Parameters.Push(id);
      Methods.Push(HttpMethod.Get);
      return await Task.FromResult(Replies.Pop() as ApiReply<TEntity>);
    }

    ///<inheritdoc/>
    public async Task<ApiReply<List<TEntity>>> GetAsync<TEntity>(IDictionary<string, object> searchParameters) where TEntity : IRootEntity, new()
    {
      Parameters.Push(searchParameters);
      Methods.Push(HttpMethod.Get);
      return await Task.FromResult(Replies.Pop() as ApiReply<List<TEntity>>);
    }

    ///<inheritdoc/>
    public async Task<ApiReply> PostAsync<TEntity>(TEntity entity) where TEntity : IRootEntity
    {
      Parameters.Push(entity);
      Methods.Push(HttpMethod.Post);
      return await Task.FromResult(Replies.Pop());
    }

    ///<inheritdoc/>
    public async Task<ApiReply<TReturn>> PostAsync<TReturn>(string route, object value)
    {
      Routes.Push(route);
      Parameters.Push(value);
      Methods.Push(HttpMethod.Post);
      return await Task.FromResult(Replies.Pop() as ApiReply<TReturn>);
    }

    ///<inheritdoc/>
    public async Task<ApiReply> PostAsync(string route, object value)
    {
      Routes.Push(route);
      Parameters.Push(value);
      Methods.Push(HttpMethod.Post);
      return await Task.FromResult(Replies.Pop());
    }

    ///<inheritdoc/>
    public async Task<ApiReply> PutAsync<TEntity>(TEntity entity) where TEntity : IRootEntity
    {
      Parameters.Push(entity);
      Methods.Push(HttpMethod.Put);
      return await Task.FromResult(Replies.Pop() as ApiReply);
    }
  }
}