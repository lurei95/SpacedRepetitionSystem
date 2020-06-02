using SpacedRepetitionSystem.Entities.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using SpacedRepetitionSystem.Entities.Entities.Security;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net;
using SpacedRepetitionSystem.Utility.Notification;
using System;
using System.IdentityModel.Tokens.Jwt;

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
    public async Task<ApiReply<TEntity>> GetAsync<TEntity>(object id) where TEntity : IRootEntity, new()
    { return await CallApi<TEntity>(HttpMethod.Get, new TEntity().Route + "/" + id.ToString(), null); }

    ///<inheritdoc/>
    public async Task<ApiReply<List<TEntity>>> GetAsync<TEntity>(IDictionary<string, object> searchParameters) where TEntity : IRootEntity, new()
    { return await CallApi<List<TEntity>>(HttpMethod.Get, new TEntity().Route, searchParameters); }

    ///<inheritdoc/>
    public async Task<ApiReply<TEntity>> PutAsync<TEntity>(TEntity entity) where TEntity : IRootEntity
    { return await CallApi<TEntity>(HttpMethod.Put, entity.Route, entity); }

    ///<inheritdoc/>
    public async Task<ApiReply> DeleteAsync<TEntity>(TEntity entity) where TEntity : IRootEntity
    { return await CallApi(HttpMethod.Delete, entity.Route, entity); }

    ///<inheritdoc/>
    public async Task<ApiReply<TEntity>> PostAsync<TEntity>(TEntity entity) where TEntity : IRootEntity
    { return await CallApi<TEntity>(HttpMethod.Post, entity.Route, entity); }

    ///<inheritdoc/>
    public async Task<ApiReply<TReturn>> PostAsync<TReturn>(string route, object value)
    { return await CallApi<TReturn>(HttpMethod.Post, route, value); }

    ///<inheritdoc/>
    public async Task<ApiReply> PostAsync(string route, object value)
    { return await CallApi(HttpMethod.Post, route, value); }

    private async Task<ApiReply> CallApi(HttpMethod method, string route, object value)
    {
      await TryRefreshAccessToken();
      return await CallApiCore(method, route, value);
    }

    private async Task<ApiReply<TReturnValue>> CallApi<TReturnValue>(HttpMethod method, string route, object value)
    {
      await TryRefreshAccessToken();
      return await CallApiCore<TReturnValue>(method, route, value);
    }

    private async Task<ApiReply> CallApiCore(HttpMethod method, string route, object value)
    {
      HttpResponseMessage response = await SendMessageAsync(method, route, value);
      return await HandleResponse(response);
    }

    private async Task<ApiReply<TReturnValue>> CallApiCore<TReturnValue>(HttpMethod method, string route, object value)
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
        request = new HttpRequestMessage(method, route);

      if (!string.IsNullOrEmpty(CurrentUser?.AccessToken))
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", CurrentUser.AccessToken);

      return await client.SendAsync(request);
    }

    private async Task<ApiReply> HandleResponse(HttpResponseMessage response)
    {
      ApiReply reply = new ApiReply();
      await HandleResponseCore(reply, response);
      return reply;
    }

    private async Task<ApiReply<TReturnValue>> HandleResponse<TReturnValue>(HttpResponseMessage response)
    {
      ApiReply<TReturnValue> reply = new ApiReply<TReturnValue>();
      await HandleResponseCore(reply, response);

      if (reply.WasSuccessful)
      {
        string content = await response.Content.ReadAsStringAsync();
        reply.Result = JsonConvert.DeserializeObject<TReturnValue>(content);
      }

      return reply;
    }

    private async Task HandleResponseCore(ApiReply reply, HttpResponseMessage response)
    {
      switch (response.StatusCode)
      {
        case HttpStatusCode.OK:
        case HttpStatusCode.Created:
        case HttpStatusCode.Accepted:
          reply.WasSuccessful = true;
          break;
        case HttpStatusCode.NotFound:
          reply.WasSuccessful = false;
          break;
        default:
          reply.WasSuccessful = false;
          await TryHandleErrorResponse(reply, response);
          break;
      }
      reply.StatusCode = response.StatusCode;
    }

    private async Task TryHandleErrorResponse(ApiReply reply, HttpResponseMessage response)
    {
      string content = await response.Content.ReadAsStringAsync();
      object result = JsonConvert.DeserializeObject(content, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
      if (result is NotifyException notifyException)
        reply.ResultMessage = notifyException.Message;
      else if (result is Exception exception)
      {
        if (exception.InnerException is NotifyException notifyException1)
          reply.ResultMessage = notifyException1.Message;
        else
          throw exception;
      }
    }

    private bool TestAccessTokenValid()
    {
      JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
      JwtSecurityToken decodedToken = tokenHandler.ReadJwtToken(CurrentUser.AccessToken);
      return decodedToken.ValidFrom < DateTime.Now && decodedToken.ValidTo > DateTime.Now;
    }

    private async Task TryRefreshAccessToken()
    {
      if (CurrentUser != null && !TestAccessTokenValid())
      {
        RefreshRequest request = new RefreshRequest()
        {
          AccessToken = CurrentUser.AccessToken,
          RefreshToken = CurrentUser.RefreshToken
        };
        ApiReply<User> reply = await CallApiCore<User>(HttpMethod.Post, "Users/RefreshToken", request);
        if (reply.WasSuccessful)
          CurrentUser = reply.Result;
      }
    }
  }
}