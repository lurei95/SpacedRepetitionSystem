using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using SpacedRepetitionSystem.Components.Middleware;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Entities.Entities.Security;
using SpacedRepetitionSystem.Utility.Notification;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Components.Tests.Middleware
{
  /// <summary>
  /// TestClass for <see cref="ApiConnector"/>
  /// </summary>
  [TestClass]
  public sealed class ApiConnectorTests
  {
    private static readonly string secretKey = "thisisasecretkeyanddontsharewithanyone";
    private readonly User user = new User() { UserId = Guid.NewGuid() };

    /// <summary>
    /// TestInitialize
    /// </summary>
    [TestInitialize]
    public void TestInitialize()
    {
      user.AccessToken = GenerateAccessToken(user.UserId, DateTime.UtcNow.AddDays(1));
      user.RefreshToken = GenerateRefreshToken().Token;
    }

    /// <summary>
    /// Tests <see cref="ApiConnector.GetAsync{TEntity}(object)"/>
    /// </summary>
    [TestMethod]
    public async Task GetSingleEntityTest()
    {    
      Card card = new Card() { CardId = 1 };
      HttpClientFactoryMock mock = CreateMock(HttpStatusCode.OK, card);
      ApiConnector connector = new ApiConnector(mock) { CurrentUser = user };

      ApiReply<Card> reply = await connector.GetAsync<Card>(1);
      TestApiReply(reply, true, null, HttpStatusCode.OK);
      Assert.AreEqual(card.CardId, reply.Result.CardId);
      TestRequest(mock, "Cards/1", HttpMethod.Get, user.AccessToken);
    }

    /// <summary>
    /// Tests that the api connector refreshes expired access tokens
    /// </summary>
    [TestMethod]
    public async Task DoesRefreshExpiredTokenTest()
    {
      User newUser = new User()
      {
        UserId = user.UserId,
        AccessToken = GenerateAccessToken(user.UserId, DateTime.Today.AddDays(1)),
        RefreshToken = GenerateRefreshToken().Token
      };
      Card card = new Card() { CardId = 1 };
      HttpClientFactoryMock mock = CreateMock<object>(HttpStatusCode.OK, null);
      ApiConnector connector = new ApiConnector(mock) { CurrentUser = user };
      user.AccessToken = GenerateAccessToken(user.UserId, DateTime.Now.AddMilliseconds(100));
      HttpResponseMessage responseMessage = new HttpResponseMessage(HttpStatusCode.OK);
      string json = JsonConvert.SerializeObject(newUser, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });
      StringContent stringContent = new StringContent(json);
      responseMessage.Content = stringContent;
      mock.ResponseMessages.Push(responseMessage);
      await Task.Delay(100);

      ApiReply reply = await connector.PutAsync(card);
      TestApiReply(reply, true, null, HttpStatusCode.OK);
      HttpRequestMessage request = TestRequest(mock, "Cards", HttpMethod.Put, newUser.AccessToken);
      string requestBody = await request.Content.ReadAsStringAsync();
      Card card1 = JsonConvert.DeserializeObject<Card>(requestBody);
      Assert.AreEqual(card.CardId, card1.CardId);

      request = TestRequest(mock, "Users/RefreshToken", HttpMethod.Post, user.AccessToken);
      requestBody = await request.Content.ReadAsStringAsync();
      RefreshRequest refreshRequest = JsonConvert.DeserializeObject<RefreshRequest>(requestBody);
      Assert.AreEqual(user.AccessToken, refreshRequest.AccessToken);
      Assert.AreEqual(user.RefreshToken, refreshRequest.RefreshToken);
      Assert.AreEqual(newUser.AccessToken, connector.CurrentUser.AccessToken);
      Assert.AreEqual(newUser.RefreshToken, connector.CurrentUser.RefreshToken);
    }

    /// <summary>
    /// Tests <see cref="ApiConnector.GetAsync{TEntity}(object)"/>
    /// </summary>
    [TestMethod]
    public async Task GetSingleEntityErrorTest()
    {
      HttpClientFactoryMock mock = CreateMock(HttpStatusCode.InternalServerError, new NotifyException("test-error"));
      ApiConnector connector = new ApiConnector(mock) { CurrentUser = user };

      ApiReply<Card> reply = await connector.GetAsync<Card>(1);
      TestApiReply(reply, false, "test-error", HttpStatusCode.InternalServerError);
      Assert.IsNull(reply.Result);
    }

    /// <summary>
    /// Tests <see cref="ApiConnector.GetAsync{TEntity}(IDictionary{string, object})"/>
    /// </summary>
    [TestMethod]
    public async Task GetEntitiesTest()
    {
      Card card = new Card() { CardId = 1 };
      List<Card> cards = new List<Card>() { card };
      HttpClientFactoryMock mock = CreateMock(HttpStatusCode.OK, cards);
      ApiConnector connector = new ApiConnector(mock) { CurrentUser = user };

      Dictionary<string, object> parameters = new Dictionary<string, object>() { { "test", 1 } };
      ApiReply<List<Card>> reply = await connector.GetAsync<Card>(parameters);
      TestApiReply(reply, true, null, HttpStatusCode.OK);
      Assert.AreEqual(card.CardId, reply.Result[0].CardId);
      HttpRequestMessage request = TestRequest(mock, "Cards", HttpMethod.Get, user.AccessToken);
      string requestBody = await request.Content.ReadAsStringAsync();
      parameters = JsonConvert.DeserializeObject<Dictionary<string, object>>(requestBody);
      Assert.AreEqual((long)1, parameters["test"]);
    }

    /// <summary>
    /// Tests <see cref="ApiConnector.GetAsync{TEntity}(IDictionary{string, object})"/>
    /// </summary>
    [TestMethod]
    public async Task GetEntitiesErrorTest()
    {
      HttpClientFactoryMock mock = CreateMock(HttpStatusCode.InternalServerError, new NotifyException("test-error"));
      ApiConnector connector = new ApiConnector(mock) { CurrentUser = user };

      Dictionary<string, object> parameters = new Dictionary<string, object>() { { "test", 1 } };
      ApiReply<List<Card>> reply = await connector.GetAsync<Card>(parameters);
      TestApiReply(reply, false, "test-error", HttpStatusCode.InternalServerError);
      Assert.IsNull(reply.Result);
    }

    /// <summary>
    /// Tests <see cref="ApiConnector.PostAsync{TEntity}(TEntity)"/>
    /// </summary>
    [TestMethod]
    public async Task PostEntityTest()
    {
      Card card = new Card() { CardId = 1 };
      HttpClientFactoryMock mock = CreateMock<object>(HttpStatusCode.OK, null);
      ApiConnector connector = new ApiConnector(mock) { CurrentUser = user };

      ApiReply reply = await connector.PostAsync(card);
      TestApiReply(reply, true, null, HttpStatusCode.OK);
      HttpRequestMessage request = TestRequest(mock, "Cards", HttpMethod.Post, user.AccessToken);
      string requestBody = await request.Content.ReadAsStringAsync();
      Card card1 = JsonConvert.DeserializeObject<Card>(requestBody);
      Assert.AreEqual(card.CardId, card1.CardId);
    }

    /// <summary>
    /// Tests <see cref="ApiConnector.PostAsync{TEntity}(TEntity)"/>
    /// </summary>
    [TestMethod]
    public async Task PostEntityErrorTest()
    {
      Card card = new Card() { CardId = 1 };
      HttpClientFactoryMock mock = CreateMock(HttpStatusCode.InternalServerError, new NotifyException("test-error"));
      ApiConnector connector = new ApiConnector(mock) { CurrentUser = user };

      ApiReply reply = await connector.PostAsync(card);
      TestApiReply(reply, false, "test-error", HttpStatusCode.InternalServerError);
    }

    /// <summary>
    /// Tests <see cref="ApiConnector.PostAsync(string, object)"/>
    /// and <see cref="ApiConnector.PostAsync{TReturn}(string, object)"/>
    /// </summary>
    [TestMethod]
    public async Task PostTest()
    {
      Card card = new Card() { CardId = 1 };
      HttpClientFactoryMock mock = CreateMock<object>(HttpStatusCode.OK, null);
      ApiConnector connector = new ApiConnector(mock) { CurrentUser = user };

      ApiReply reply = await connector.PostAsync("test", card);
      TestApiReply(reply, true, null, HttpStatusCode.OK);
      HttpRequestMessage request = TestRequest(mock, "test", HttpMethod.Post, user.AccessToken);
      string requestBody = await request.Content.ReadAsStringAsync();
      Card card1 = JsonConvert.DeserializeObject<Card>(requestBody);
      Assert.AreEqual(card.CardId, card1.CardId);

      mock = CreateMock(HttpStatusCode.OK, card);
      connector = new ApiConnector(mock) { CurrentUser = user };
      ApiReply<Card> reply1 = await connector.PostAsync<Card>("test", card);
      TestApiReply(reply, true, null, HttpStatusCode.OK);
      Assert.AreEqual(1, reply1.Result.CardId);
      request = TestRequest(mock, "test", HttpMethod.Post, user.AccessToken);
      requestBody = await request.Content.ReadAsStringAsync();
      card1 = JsonConvert.DeserializeObject<Card>(requestBody);
      Assert.AreEqual(card.CardId, card1.CardId);
    }

    /// <summary>
    /// Tests <see cref="ApiConnector.PostAsync(string, object)"/>
    /// and <see cref="ApiConnector.PostAsync{TReturn}(string, object)"/>
    /// </summary>
    [TestMethod]
    public async Task PostErrorTest()
    {
      Card card = new Card() { CardId = 1 };
      HttpClientFactoryMock mock = CreateMock(HttpStatusCode.InternalServerError, new NotifyException("test-error"));
      ApiConnector connector = new ApiConnector(mock) { CurrentUser = user };
      ApiReply reply = await connector.PostAsync("test", card);
      TestApiReply(reply, false, "test-error", HttpStatusCode.InternalServerError);

      mock = CreateMock(HttpStatusCode.InternalServerError, new NotifyException("test-error"));
      connector = new ApiConnector(mock) { CurrentUser = user };
      ApiReply<Card> reply1 = await connector.PostAsync<Card>("test", card);
      TestApiReply(reply, false, "test-error", HttpStatusCode.InternalServerError);
      Assert.IsNull(reply1.Result);
    }

    /// <summary>
    /// Tests <see cref="ApiConnector.PostAsync{TEntity}(TEntity)"/>
    /// </summary>
    [TestMethod]
    public async Task PutEntityTest()
    {
      Card card = new Card() { CardId = 1 };
      HttpClientFactoryMock mock = CreateMock<object>(HttpStatusCode.OK, null);
      ApiConnector connector = new ApiConnector(mock) { CurrentUser = user };

      ApiReply reply = await connector.PutAsync(card);
      TestApiReply(reply, true, null, HttpStatusCode.OK);
      HttpRequestMessage request = TestRequest(mock, "Cards", HttpMethod.Put, user.AccessToken);
      string requestBody = await request.Content.ReadAsStringAsync();
      Card card1 = JsonConvert.DeserializeObject<Card>(requestBody);
      Assert.AreEqual(card.CardId, card1.CardId);
    }

    /// <summary>
    /// Tests <see cref="ApiConnector.PostAsync{TEntity}(TEntity)"/>
    /// </summary>
    [TestMethod]
    public async Task PutEntityErrorTest()
    {
      Card card = new Card() { CardId = 1 };
      HttpClientFactoryMock mock = CreateMock(HttpStatusCode.InternalServerError, new NotifyException("test-error"));
      ApiConnector connector = new ApiConnector(mock) { CurrentUser = user };

      ApiReply reply = await connector.PutAsync(card);
      TestApiReply(reply, false, "test-error", HttpStatusCode.InternalServerError);
    }

    /// <summary>
    /// Tests <see cref="ApiConnector.DeleteAsync{TEntity}(TEntity)"/>
    /// </summary>
    [TestMethod]
    public async Task DeleteEntityTest()
    {
      Card card = new Card() { CardId = 1 };
      HttpClientFactoryMock mock = CreateMock<object>(HttpStatusCode.OK, null);
      ApiConnector connector = new ApiConnector(mock) { CurrentUser = user };

      ApiReply reply = await connector.DeleteAsync(card);
      TestApiReply(reply, true, null, HttpStatusCode.OK);
      HttpRequestMessage request = TestRequest(mock, "Cards", HttpMethod.Delete, user.AccessToken);   
      string requestBody = await request.Content.ReadAsStringAsync();
      Card card1 = JsonConvert.DeserializeObject<Card>(requestBody);
      Assert.AreEqual(card.CardId, card1.CardId);
    }

    /// <summary>
    /// Tests <see cref="ApiConnector.DeleteAsync{TEntity}(TEntity)"/>
    /// </summary>
    [TestMethod]
    public async Task DeleteEntityErrorTest()
    {
      Card card = new Card() { CardId = 1 };
      HttpClientFactoryMock mock = CreateMock(HttpStatusCode.InternalServerError, new NotifyException("test-error"));
      ApiConnector connector = new ApiConnector(mock) { CurrentUser = user };

      ApiReply reply = await connector.DeleteAsync(card);
      TestApiReply(reply, false, "test-error", HttpStatusCode.InternalServerError);
    }

    private static string GenerateAccessToken(Guid userId, DateTime expiredDate)
    {
      JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
      byte[] key = Encoding.ASCII.GetBytes(secretKey);
      SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
      {
        Subject = new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, Convert.ToString(userId)) }),
        Expires = expiredDate,
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
          SecurityAlgorithms.HmacSha256Signature)
      };
      SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
      return tokenHandler.WriteToken(token);
    }

    private HttpRequestMessage TestRequest(HttpClientFactoryMock mock, string address, HttpMethod method, string accessToken)
    {
      HttpRequestMessage request = mock.RequestMessages.Pop();
      Assert.AreEqual(mock.BaseAddress + address, request.RequestUri.ToString());
      Assert.AreEqual(method, request.Method);
      Assert.AreEqual(accessToken, request.Headers.Authorization.Parameter);
      return request;
    }

    private void TestApiReply(ApiReply reply, bool successful, string expectedMessage, HttpStatusCode statusCode)
    {
      Assert.AreEqual(successful, reply.WasSuccessful);
      Assert.AreEqual(expectedMessage, reply.ResultMessage);
      Assert.AreEqual(statusCode, reply.StatusCode);
    }

    private HttpClientFactoryMock CreateMock<TContent>(HttpStatusCode statusCode, TContent content)
    {
      HttpResponseMessage responseMessage = new HttpResponseMessage(statusCode);
      HttpClientFactoryMock mock = new HttpClientFactoryMock();
      if (content != null)
      {
        string json = JsonConvert.SerializeObject(content, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });
        StringContent stringContent = new StringContent(json);
        responseMessage.Content = stringContent;
      }
      mock.ResponseMessages.Push(responseMessage);
      return mock;
    }

    private static RefreshToken GenerateRefreshToken()
    {
      RefreshToken refreshToken = new RefreshToken();

      byte[] randomNumber = new byte[32];
      using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
      {
        rng.GetBytes(randomNumber);
        refreshToken.Token = Convert.ToBase64String(randomNumber);
      }
      refreshToken.ExpirationDate = DateTime.Today.AddDays(1);
      return refreshToken;
    }
  }
}