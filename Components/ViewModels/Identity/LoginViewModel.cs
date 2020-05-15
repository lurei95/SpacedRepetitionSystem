using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using SpacedRepetitionSystem.Entities;
using SpacedRepetitionSystem.Entities.Entities.Users;
using SpacedRepetitionSystem.Entities.Validation.Core;
using SpacedRepetitionSystem.Logic.Controllers.Core;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Components.ViewModels.Identity
{
  /// <summary>
  /// ViewModel for a login page
  /// </summary>
  public sealed class LoginViewModel : IdentityViewModelBase
  {
    private ClaimsPrincipal claimsPrincipal;

    /// <summary>
    /// The authentication state task
    /// </summary>
    public Task<AuthenticationState> AuthenticationStateTask { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="navigationManager">NavigationManager (Injected)</param>
    /// <param name="authenticationStateProvider">AuthenticationStateProvider (Injected)</param>
    /// <param name="apiConnector">API Connector (Injected)</param>
    /// <param name="changeValidator">EntityChangeValdiator (Injected)</param>
    public LoginViewModel(NavigationManager navigationManager, AuthenticationStateProvider authenticationStateProvider, 
      IApiConnector apiConnector, EntityChangeValidator<User> changeValidator) 
      : base(navigationManager, authenticationStateProvider, apiConnector, changeValidator)
    { }

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
    protected override async Task SubmitAsyncCore()
    {
;
      User returnedUser = await ApiConnector.Login(User.Email, User.Password);
      if (returnedUser != null)
      {
        await AuthenticationStateProvider.MarkUserAsAuthenticated(returnedUser);
        NavigationManager.NavigateTo("/");
      }
      else
        ErrorMessage = Errors.InvalidUserNameOrPassword;
    }
  }
}