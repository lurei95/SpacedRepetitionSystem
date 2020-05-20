using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using SpacedRepetitionSystem.Components.Middleware;
using SpacedRepetitionSystem.Entities.Entities.Security;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;

namespace SpacedRepetitionSystem.ViewModels.Identity
{
  /// <summary>
  /// Custom implemenntatio of <see cref="AuthenticationStateProvider"/>
  /// </summary>
  public sealed class CustomAuthenticationStateProvider : AuthenticationStateProvider
  {
    private readonly ILocalStorageService localStorageService;
    private readonly IApiConnector apiConnector;
    private readonly NavigationManager navigationManager;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="localStorageService">LocalStorageService (injected)</param>
    /// <param name="apiConnector">ApiConnector (injected)</param>
    /// <param name="navigationManager">NavigationManager (injected)</param>
    public CustomAuthenticationStateProvider(ILocalStorageService localStorageService, IApiConnector apiConnector, NavigationManager navigationManager)
    {
      this.navigationManager = navigationManager;
      this.apiConnector = apiConnector;
      this.localStorageService = localStorageService;
    }

    ///<inheritdoc/>
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
      string accessToken = await localStorageService.GetItemAsync<string>("accessToken");
      string refreshToken = await localStorageService.GetItemAsync<string>("refreshToken");
      ClaimsIdentity identity = new ClaimsIdentity();
      apiConnector.CurrentUser = null;

      if (!string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(refreshToken))
      {
        ApiReply<User> reply = await apiConnector.PostAsync<User>("Users/GetUserByAccessToken", accessToken);

        if (reply.WasSuccessful)
        {
          User user = reply.Result;
          user.AccessToken = accessToken;
          user.RefreshToken = refreshToken;
          apiConnector.CurrentUser = reply.Result;
          identity = GetClaimsIdentity(reply.Result);
        }
        else
          apiConnector.CurrentUser = null;
      }

      if (apiConnector.CurrentUser == null)
        navigationManager.NavigateTo("/Login");

      ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);
      return await Task.FromResult(new AuthenticationState(claimsPrincipal));
    }

    /// <summary>
    /// Marks an user as authenticated
    /// </summary>
    /// <param name="user">The user</param>
    /// <returns></returns>
    public async Task MarkUserAsAuthenticated(User user)
    {
      await localStorageService.SetItemAsync("accessToken", user.AccessToken);
      await localStorageService.SetItemAsync("refreshToken", user.RefreshToken);
      ClaimsIdentity identity = GetClaimsIdentity(user);
      ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);
      NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
      apiConnector.CurrentUser = user;
      navigationManager.NavigateTo("/");
    }

    /// <summary>
    /// Marks a user as logged out
    /// </summary>
    /// <returns></returns>
    public async Task MarkUserAsLoggedOut()
    {
      await localStorageService.RemoveItemAsync("refreshToken");
      await localStorageService.RemoveItemAsync("accessToken");
      ClaimsIdentity identity = new ClaimsIdentity();
      ClaimsPrincipal principal = new ClaimsPrincipal(identity);
      apiConnector.CurrentUser = null;
      navigationManager.NavigateTo("/Login");
      NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
    }

    private ClaimsIdentity GetClaimsIdentity(User user)
    {
      ClaimsIdentity claimsIdentity = new ClaimsIdentity();
      if (user?.Email != null)
        claimsIdentity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, user.Email) }, "apiauth_type");
      return claimsIdentity;
    }
  }
}