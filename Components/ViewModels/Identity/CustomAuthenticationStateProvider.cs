using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using SpacedRepetitionSystem.Entities.Entities.Users;
using SpacedRepetitionSystem.Logic.Controllers.Core;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Components.ViewModels.Identity
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
    /// <param name="context">Context (Injected)</param>
    public CustomAuthenticationStateProvider(ILocalStorageService localStorageService, IApiConnector apiConnector, DbContext context, NavigationManager navigationManager)
    {
      this.navigationManager = navigationManager;
      this.apiConnector = apiConnector;
      this.apiConnector.Context = context;
      this.localStorageService = localStorageService;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
      string accessToken = await localStorageService.GetItemAsync<string>("accessToken");
      ClaimsIdentity identity;
      if (accessToken != null && accessToken != string.Empty)
      {
        User user = await apiConnector.GetUserByAccessTokenAsync(accessToken);
        identity = GetClaimsIdentity(user);
      }
      else
      {
        identity = new ClaimsIdentity();
        navigationManager.NavigateTo("/Login");
      }

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
      NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
      navigationManager.NavigateTo("/Login");
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