using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using SpacedRepetitionSystem.Components.Middleware;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Entities.Entities.Security;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
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
    private User user = new User() { UserId = Guid.NewGuid() };

    /// <summary>
    /// Tests <see cref="ApiConnector.GetAsync{TEntity}(object)"/>
    /// </summary>
    [TestMethod]
    public async Task GetSingleEntityTest()
    {
      user.AccessToken = GenerateAccessToken(user.UserId);
      Card card = new Card() { CardId = 1 };
      HttpClientFactoryMock mock = new HttpClientFactoryMock
      { ResponseMessage = new HttpResponseMessage(HttpStatusCode.OK) };
      StringContent stringContent = new StringContent(JsonConvert.SerializeObject(card));
      mock.ResponseMessage.Content = stringContent;
      ApiConnector connector = new ApiConnector(mock) { CurrentUser = user };

      ApiReply<Card> reply = await connector.GetAsync<Card>(1);
      Assert.IsTrue(reply.WasSuccessful);
      Assert.IsTrue(string.IsNullOrEmpty(reply.ResultMessage));
      Assert.AreEqual(card.CardId, reply.Result.CardId);
      Assert.AreEqual(mock.BaseAddress + "Cards/1", mock.RequestMessage.RequestUri.ToString());
      Assert.AreEqual(mock.RequestMessage.Method, HttpMethod.Get);
      Assert.AreEqual(user.AccessToken, mock.RequestMessage.Headers.Authorization.Parameter);
    }

    /// <summary>
    /// Tests <see cref="ApiConnector.GetAsync{TEntity}(IDictionary{string, object})"/>
    /// </summary>
    [TestMethod]
    public async Task GetEntitiesTest()
    {
      user.AccessToken = GenerateAccessToken(user.UserId);
      Card card = new Card() { CardId = 1 };
      List<Card> cards = new List<Card>() { card };
      HttpClientFactoryMock mock = new HttpClientFactoryMock
      { ResponseMessage = new HttpResponseMessage(HttpStatusCode.OK) };
      StringContent stringContent = new StringContent(JsonConvert.SerializeObject(cards));
      mock.ResponseMessage.Content = stringContent;
      ApiConnector connector = new ApiConnector(mock) { CurrentUser = user };

      Dictionary<string, object> parameters = new Dictionary<string, object>() { { "test", 1 } };
      ApiReply<List<Card>> reply = await connector.GetAsync<Card>(parameters);
      Assert.IsTrue(reply.WasSuccessful);
      Assert.IsTrue(string.IsNullOrEmpty(reply.ResultMessage));
      Assert.AreEqual(card.CardId, reply.Result[0].CardId);
      Assert.AreEqual(mock.BaseAddress + "Cards", mock.RequestMessage.RequestUri.ToString());
      Assert.AreEqual(mock.RequestMessage.Method, HttpMethod.Get);
      Assert.AreEqual(user.AccessToken, mock.RequestMessage.Headers.Authorization.Parameter);
      string requestBody = await mock.RequestMessage.Content.ReadAsStringAsync();
      parameters = JsonConvert.DeserializeObject<Dictionary<string, object>>(requestBody);
      Assert.AreEqual((long)1, parameters["test"]);
    }

    /// <summary>
    /// Tests <see cref="ApiConnector.PostAsync{TEntity}(TEntity)"/>
    /// </summary>
    [TestMethod]
    public async Task PostEntityTest()
    {
      user.AccessToken = GenerateAccessToken(user.UserId);
      Card card = new Card() { CardId = 1 };
      HttpClientFactoryMock mock = new HttpClientFactoryMock
      { ResponseMessage = new HttpResponseMessage(HttpStatusCode.OK) };
      ApiConnector connector = new ApiConnector(mock) { CurrentUser = user };

      ApiReply reply = await connector.PostAsync(card);
      Assert.IsTrue(reply.WasSuccessful);
      Assert.IsTrue(string.IsNullOrEmpty(reply.ResultMessage));
      Assert.AreEqual(mock.BaseAddress + "Cards", mock.RequestMessage.RequestUri.ToString());
      Assert.AreEqual(mock.RequestMessage.Method, HttpMethod.Post);
      Assert.AreEqual(user.AccessToken, mock.RequestMessage.Headers.Authorization.Parameter);
      string requestBody = await mock.RequestMessage.Content.ReadAsStringAsync();
      Card card1 = JsonConvert.DeserializeObject<Card>(requestBody);
      Assert.AreEqual(card.CardId, card1.CardId);
    }

    /// <summary>
    /// Tests <see cref="ApiConnector.PostAsync(string, object)"/>
    /// and <see cref="ApiConnector.PostAsync{TReturn}(string, object)"/>
    /// </summary>
    [TestMethod]
    public async Task PostTest()
    {
      user.AccessToken = GenerateAccessToken(user.UserId);
      Card card = new Card() { CardId = 1 };
      HttpClientFactoryMock mock = new HttpClientFactoryMock
      { ResponseMessage = new HttpResponseMessage(HttpStatusCode.OK) };
      ApiConnector connector = new ApiConnector(mock) { CurrentUser = user };

      ApiReply reply = await connector.PostAsync("test", card);
      Assert.IsTrue(reply.WasSuccessful);
      Assert.IsTrue(string.IsNullOrEmpty(reply.ResultMessage));
      Assert.AreEqual(mock.BaseAddress + "test", mock.RequestMessage.RequestUri.ToString());
      Assert.AreEqual(mock.RequestMessage.Method, HttpMethod.Post);
      Assert.AreEqual(user.AccessToken, mock.RequestMessage.Headers.Authorization.Parameter);
      string requestBody = await mock.RequestMessage.Content.ReadAsStringAsync();
      Card card1 = JsonConvert.DeserializeObject<Card>(requestBody);
      Assert.AreEqual(card.CardId, card1.CardId);

      StringContent stringContent = new StringContent(JsonConvert.SerializeObject(card));
      mock.ResponseMessage.Content = stringContent;
      ApiReply<Card> reply1 = await connector.PostAsync<Card>("test", card);
      Assert.IsTrue(reply1.WasSuccessful);
      Assert.IsTrue(string.IsNullOrEmpty(reply1.ResultMessage));
      Assert.AreEqual(1, reply1.Result.CardId);
      Assert.AreEqual(mock.BaseAddress + "test", mock.RequestMessage.RequestUri.ToString());
      Assert.AreEqual(mock.RequestMessage.Method, HttpMethod.Post);
      Assert.AreEqual(user.AccessToken, mock.RequestMessage.Headers.Authorization.Parameter);
      requestBody = await mock.RequestMessage.Content.ReadAsStringAsync();
      card1 = JsonConvert.DeserializeObject<Card>(requestBody);
      Assert.AreEqual(card.CardId, card1.CardId);
    }

    /// <summary>
    /// Tests <see cref="ApiConnector.PostAsync{TEntity}(TEntity)"/>
    /// </summary>
    [TestMethod]
    public async Task PutEntityTest()
    {
      user.AccessToken = GenerateAccessToken(user.UserId);
      Card card = new Card() { CardId = 1 };
      HttpClientFactoryMock mock = new HttpClientFactoryMock
      { ResponseMessage = new HttpResponseMessage(HttpStatusCode.OK) };
      ApiConnector connector = new ApiConnector(mock) { CurrentUser = user };

      ApiReply reply = await connector.PutAsync(card);
      Assert.IsTrue(reply.WasSuccessful);
      Assert.IsTrue(string.IsNullOrEmpty(reply.ResultMessage));
      Assert.AreEqual(mock.BaseAddress + "Cards", mock.RequestMessage.RequestUri.ToString());
      Assert.AreEqual(mock.RequestMessage.Method, HttpMethod.Put);
      Assert.AreEqual(user.AccessToken, mock.RequestMessage.Headers.Authorization.Parameter);
      string requestBody = await mock.RequestMessage.Content.ReadAsStringAsync();
      Card card1 = JsonConvert.DeserializeObject<Card>(requestBody);
      Assert.AreEqual(card.CardId, card1.CardId);
    }

    /// <summary>
    /// Tests <see cref="ApiConnector.DeleteAsync{TEntity}(TEntity)"/>
    /// </summary>
    [TestMethod]
    public async Task DeleteEntityTest()
    {
      user.AccessToken = GenerateAccessToken(user.UserId);
      Card card = new Card() { CardId = 1 };
      HttpClientFactoryMock mock = new HttpClientFactoryMock
      { ResponseMessage = new HttpResponseMessage(HttpStatusCode.OK) };
      ApiConnector connector = new ApiConnector(mock) { CurrentUser = user };

      ApiReply reply = await connector.DeleteAsync(card);
      Assert.IsTrue(reply.WasSuccessful);
      Assert.IsTrue(string.IsNullOrEmpty(reply.ResultMessage));
      Assert.AreEqual(mock.BaseAddress + "Cards", mock.RequestMessage.RequestUri.ToString());
      Assert.AreEqual(mock.RequestMessage.Method, HttpMethod.Delete);
      Assert.AreEqual(user.AccessToken, mock.RequestMessage.Headers.Authorization.Parameter);
      string requestBody = await mock.RequestMessage.Content.ReadAsStringAsync();
      Card card1 = JsonConvert.DeserializeObject<Card>(requestBody);
      Assert.AreEqual(card.CardId, card1.CardId);
    }

    private static string GenerateAccessToken(Guid userId)
    {
      JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
      byte[] key = Encoding.ASCII.GetBytes(secretKey);
      SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
      {
        Subject = new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, Convert.ToString(userId)) }),
        Expires = DateTime.UtcNow.AddDays(1),
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
          SecurityAlgorithms.HmacSha256Signature)
      };
      SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
      return tokenHandler.WriteToken(token);
    }
  }
}
