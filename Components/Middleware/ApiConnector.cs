using SpacedRepetitionSystem.Entities.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using SpacedRepetitionSystem.Entities.Entities.Security;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace SpacedRepetitionSystem.Components.Middleware
{
  /// <summary>
  /// Implementation of <see cref="IApiConnector"/>
  /// </summary>
  public sealed class ApiConnector : IApiConnector
  {
    private readonly IHttpClientFactory httpClientFactory;

    /// <summary>
    /// The current user
    /// </summary>
    public User CurrentUser { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="httpClientFactory">HTTPClientFactory (Injected)</param>
    public ApiConnector(IHttpClientFactory httpClientFactory)
    { this.httpClientFactory = httpClientFactory; }

    ///<inheritdoc/>
    public async Task<TEntity> GetAsync<TEntity>(object id) where TEntity : IRootEntity, new()
    { return await CallApiCore<TEntity>(HttpMethod.Get, new TEntity().Route + "/" + id.ToString(), null); }

    ///<inheritdoc/>
    public async Task<List<TEntity>> GetAsync<TEntity>(IDictionary<string, object> searchParameters) where TEntity : IRootEntity, new()
    { return await CallApiCore<List<TEntity>>(HttpMethod.Get, new TEntity().Route, searchParameters); }

    ///<inheritdoc/>
    public async Task<bool> PutAsync<TEntity>(TEntity entity) where TEntity : IRootEntity
    {
      await CallApiCore(HttpMethod.Put, entity.Route, entity);
      return true;
    }

    ///<inheritdoc/>
    public async Task<bool> DeleteAsync<TEntity>(TEntity entity) where TEntity : IRootEntity
    {
      await CallApiCore(HttpMethod.Delete, entity.Route, entity);
      return true;
    }

    ///<inheritdoc/>
    public async Task<bool> PostAsync<TEntity>(TEntity entity) where TEntity : IRootEntity
    {
      await CallApiCore(HttpMethod.Post, entity.Route, entity);
      return true;
    }

    ///<inheritdoc/>
    public async Task<TReturn> PostAsync<TReturn>(string route, object value)
    { return await CallApiCore<TReturn>(HttpMethod.Post, route, value); }

    ///<inheritdoc/>
    public async Task PostAsync(string route, object value)
    { await CallApiCore(HttpMethod.Post, route, value);  }

    private async Task CallApiCore(HttpMethod method, string route, object value)
    {
      HttpResponseMessage response = await SendMessageAsync(method, route, value);
      await HandleResponse(response);
    }

    private async Task<TReturnValue> CallApiCore<TReturnValue>(HttpMethod method, string route, object value)
    {
      HttpResponseMessage response = await SendMessageAsync(method, route, value);
      return await HandleResponse<TReturnValue>(response);
    }

    private async Task<HttpResponseMessage> SendMessageAsync(HttpMethod method, string route, object value)
    {
      HttpClient client = httpClientFactory.CreateClient("Default");
      client.DefaultRequestHeaders.Add("User-Agent", "BlazorServer");

      HttpRequestMessage request;
      if (value != null)
      {
        string serialized = JsonConvert.SerializeObject(value);
        request = new HttpRequestMessage(method, route) { Content = new StringContent(serialized) };
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
      }
      else
        request = new HttpRequestMessage();

      if (!string.IsNullOrEmpty(CurrentUser?.AccessToken))
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", CurrentUser.AccessToken);

      return await client.SendAsync(request);
    }

    private async Task HandleResponse(HttpResponseMessage response)
    {

    }

    private async Task<TReturnValue> HandleResponse<TReturnValue>(HttpResponseMessage response)
    {
      string content = await response.Content.ReadAsStringAsync();
      return JsonConvert.DeserializeObject<TReturnValue>(content);
    }
  }
}