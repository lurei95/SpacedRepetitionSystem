using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using SpacedRepetitionSystem.Entities.Entities.Users;
using SpacedRepetitionSystem.Logic.Controllers.Core;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Components.ViewModels.Identity
{
  /// <summary>
  /// ViewModel for a signup page
  /// </summary>
  public sealed class SignupViewModel : PageViewModelBase
  {
    private readonly AuthenticationStateProvider authenticationStateProvider;
    private readonly IApiConnector apiConnector;

    /// <summary>
    /// The Login Message
    /// </summary>
    public string LoginMesssage { get; set; }

    /// <summary>
    /// The user
    /// </summary>
    public User User { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="navigationManager">NavigationManager (Injected)</param>
    /// <param name="authenticationStateProvider">AuthenticationStateProvider (Injected)</param>

    public SignupViewModel(NavigationManager navigationManager, AuthenticationStateProvider authenticationStateProvider, 
      IApiConnector apiConnector) : base(navigationManager)
    { 
      User = new User();
      this.authenticationStateProvider = authenticationStateProvider;
      this.apiConnector = apiConnector;
    }

    /// <summary>
    /// Tries to log in
    /// </summary>
    /// <returns>Task with result</returns>
    public async Task<bool> RegisterUser()
    {
      if (apiConnector.Post(User))
      {
        await (authenticationStateProvider as CustomAuthenticationStateProvider).MarkUserAsAuthenticated(User);
        NavigationManager.NavigateTo("/");
      }
      else
        LoginMesssage = "Invalid username or password";
      return await Task.FromResult(true);
    }
  }
}