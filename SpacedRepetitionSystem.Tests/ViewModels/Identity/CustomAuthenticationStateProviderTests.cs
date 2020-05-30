using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpacedRepetitionSystem.Components.Middleware;
using SpacedRepetitionSystem.Components.Tests;
using SpacedRepetitionSystem.Entities.Entities.Security;
using SpacedRepetitionSystem.ViewModels.Identity;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Tests.ViewModels.Identity
{
  /// <summary>
  /// Testclass for <see cref="CustomAuthenticationStateProvider"/>
  /// </summary>
  [TestClass]
  public sealed class CustomAuthenticationStateProviderTests
  {
    private static readonly string secretKey = "thisisasecretkeyanddontsharewithanyone";
    private readonly NavigationManagerMock navigationManagerMock = new NavigationManagerMock();
    private User user;

    /// <summary>
    /// Initializes the tests
    /// </summary>
    [TestInitialize]
    public void TestInitialize()
    {
      user = new User()
      {
        UserId = Guid.NewGuid(),
        Email = "test@test.com",
        RefreshToken = GenerateRefreshToken(DateTime.Today.AddDays(1)).Token
      };
      user.AccessToken = GenerateAccessToken(user.UserId);
    }

    /// <summary>
    /// Tests successful authentication with <see cref="CustomAuthenticationStateProvider.GetAuthenticationStateAsync"/>
    /// </summary>
    [TestMethod]
    public async Task GetAuthenticationStateSuccessfullyTest()
    { await GetAuthenticationStateTestCore(true, true); }

    /// <summary>
    /// Tests failed authentication with <see cref="CustomAuthenticationStateProvider.GetAuthenticationStateAsync"/>
    /// </summary>
    [TestMethod]
    public async Task GetAuthenticationStateFailedTest()
    { await GetAuthenticationStateTestCore(false, true); }

    /// <summary>
    /// Tests <see cref="CustomAuthenticationStateProvider.GetAuthenticationStateAsync"/> without tokens in local storage
    /// </summary>
    [TestMethod]
    public async Task GetAuthenticationStateWithoutTokenInStorageTest()
    { await GetAuthenticationStateTestCore(false, false); }

    /// <summary>
    /// Tests <see cref="CustomAuthenticationStateProvider.MarkUserAsAuthenticated(User)"/>
    /// </summary>
    [TestMethod]
    public async Task MarkUserAsAuthenticatedTest()
    {
      LocalStorageServiceMock localStorageService = new LocalStorageServiceMock();
      ApiConnectorMock mock = new ApiConnectorMock();
      CustomAuthenticationStateProvider authenticationStateProvider
        = new CustomAuthenticationStateProvider(localStorageService, mock, navigationManagerMock);
      bool wasStateChanged = false;
      authenticationStateProvider.AuthenticationStateChanged += async (result) =>
      {
        wasStateChanged = true;
        AuthenticationState state = await result;
        Assert.IsNotNull(state.User);
        Assert.AreEqual(user.UserId.ToString(), state.User.Identity.Name);
      };
      await authenticationStateProvider.MarkUserAsAuthenticated(user);

      Assert.IsTrue(wasStateChanged);
      Assert.AreSame(user, mock.CurrentUser);
      Assert.AreEqual(user.AccessToken, localStorageService.SetItems["accessToken"]);
      Assert.AreEqual(user.RefreshToken, localStorageService.SetItems["refreshToken"]);
    }

    /// <summary>
    /// Tests <see cref="CustomAuthenticationStateProvider.MarkUserAsLoggedOut"/>
    /// </summary>
    [TestMethod]
    public async Task MarkUserAsLoggedOutTest()
    {
      LocalStorageServiceMock localStorageService = new LocalStorageServiceMock();
      ApiConnectorMock mock = new ApiConnectorMock();
      CustomAuthenticationStateProvider authenticationStateProvider
        = new CustomAuthenticationStateProvider(localStorageService, mock, navigationManagerMock);
      bool wasStateChanged = false;
      authenticationStateProvider.AuthenticationStateChanged += async (result) =>
      {
        wasStateChanged = true;
        AuthenticationState state = await result;
        Assert.IsNull(state.User.Identity.Name);
      };
      await authenticationStateProvider.MarkUserAsAuthenticated(user);
      await authenticationStateProvider.MarkUserAsLoggedOut();

      Assert.IsTrue(wasStateChanged);
      Assert.IsNull(mock.CurrentUser);
      Assert.IsFalse(localStorageService.SetItems.ContainsKey("accessToken"));
      Assert.IsFalse(localStorageService.SetItems.ContainsKey("refreshToken"));
    }

    private async Task GetAuthenticationStateTestCore(bool replySuccessful, bool hasTokensInStorage)
    {
      LocalStorageServiceMock localStorageService = new LocalStorageServiceMock();
      if (hasTokensInStorage)
      {
        localStorageService.GetItemResults.Add("accessToken", user.AccessToken);
        localStorageService.GetItemResults.Add("refreshToken", user.RefreshToken);
      }
      ApiConnectorMock mock = new ApiConnectorMock();
      mock.Replies.Push(new ApiReply<User>()
      {
        WasSuccessful = replySuccessful,
        Result = replySuccessful ? user : null
      });
      CustomAuthenticationStateProvider authenticationStateProvider
        = new CustomAuthenticationStateProvider(localStorageService, mock, navigationManagerMock);
      bool wasStateChanged = false;
      authenticationStateProvider.AuthenticationStateChanged += async (result) =>
      {
        wasStateChanged = true;
        AuthenticationState state = await result;
        if (replySuccessful && hasTokensInStorage)
        {
          Assert.IsNotNull(state.User);
          Assert.AreEqual(user.UserId.ToString(), state.User.Identity.Name);
        }
        else
          Assert.IsNull(state.User.Identity.Name);
      };
      AuthenticationState state = await authenticationStateProvider.GetAuthenticationStateAsync();

      Assert.IsTrue(wasStateChanged);
      if (replySuccessful && hasTokensInStorage)
      {
        Assert.AreSame(user, mock.CurrentUser);
        Assert.IsNotNull(state.User);
        Assert.AreEqual(user.UserId.ToString(), state.User.Identity.Name);
      }
      else
      {
        Assert.IsNull(state.User.Identity.Name);
        Assert.IsNull(mock.CurrentUser);
      }

      if (hasTokensInStorage)
      {
        Assert.AreEqual(user.AccessToken, mock.Parameters.Pop());
        Assert.AreEqual("Users/GetUserByAccessToken", mock.Routes.Pop());
        Assert.AreEqual(HttpMethod.Post, mock.Methods);
      }

      if (!hasTokensInStorage || !replySuccessful)
        Assert.AreEqual("/Login", navigationManagerMock);
    }

    private string GenerateAccessToken(Guid userId)
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

    private static RefreshToken GenerateRefreshToken(DateTime expirationDate)
    {
      RefreshToken refreshToken = new RefreshToken();

      byte[] randomNumber = new byte[32];
      using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
      {
        rng.GetBytes(randomNumber);
        refreshToken.Token = Convert.ToBase64String(randomNumber);
      }
      refreshToken.ExpirationDate = expirationDate;
      return refreshToken;
    }

  }
}