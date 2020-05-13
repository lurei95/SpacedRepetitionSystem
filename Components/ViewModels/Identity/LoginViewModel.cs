using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using SpacedRepetitionSystem.Entities.Entities.Users;
using SpacedRepetitionSystem.Logic.Controllers.Core;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Components.ViewModels.Identity
{
  /// <summary>
  /// ViewModel for a login page
  /// </summary>
  public sealed class LoginViewModel : PageViewModelBase
  {
    private readonly AuthenticationStateProvider authenticationStateProvider;
    private readonly IApiConnector apiConnector;
    private ClaimsPrincipal claimsPrincipal;

    /// <summary>
    /// Whether the ViewModel is currently logging in
    /// </summary>
    public bool IsLoggingIn { get; set; }

    /// <summary>
    /// The Login Message
    /// </summary>
    public string LoginMesssage { get; set; }

    /// <summary>
    /// The user
    /// </summary>
    public User User { get; set; }

    /// <summary>
    /// The authentication state task
    /// </summary>
    public Task<AuthenticationState> AuthenticationStateTask { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="navigationManager">NavigationManager (Injected)</param>
    /// <param name="authenticationStateProvider">AuthenticationStateProvider (Injected)</param>
    public LoginViewModel(NavigationManager navigationManager, AuthenticationStateProvider authenticationStateProvider, IApiConnector apiConnector) : base(navigationManager)
    {
      this.authenticationStateProvider = authenticationStateProvider;
      this.apiConnector = apiConnector;
    }

    ///<inheritdoc/>
    public async override Task InitializeAsync()
    {
      User = new User();

      claimsPrincipal = (await AuthenticationStateTask).User;

      if (claimsPrincipal.Identity.IsAuthenticated)
        NavigationManager.NavigateTo("/");
    }

    /// <summary>
    /// Tries to log in
    /// </summary>
    /// <returns>Task with result</returns>
    public async Task<bool> TryLogIn()
    {
      IsLoggingIn = true;
      User returnedUser = await apiConnector.Login(User.UserId, User.Password);
      if (returnedUser != null)
      {
        await (authenticationStateProvider as CustomAuthenticationStateProvider).MarkUserAsAuthenticated(returnedUser);
        NavigationManager.NavigateTo("/");
      }
      else
        LoginMesssage = "Invalid username or password";

      IsLoggingIn = false;
      return true;
    }
  }
}