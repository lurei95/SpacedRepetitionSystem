using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using SpacedRepetitionSystem.Components.Commands;
using SpacedRepetitionSystem.Components.Middleware;
using SpacedRepetitionSystem.Entities;
using SpacedRepetitionSystem.Entities.Entities.Security;
using SpacedRepetitionSystem.Entities.Validation.Core;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.ViewModels.Identity
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
    /// Command for loging in as guest user
    /// </summary>
    public Command LoginAsGuestUserCommand { get; private set; }

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
    {
      LoginAsGuestUserCommand = new Command()
      {
        ToolTip = Messages.LoginAsGuestUserCommandToolTip,
        CommandText = Messages.LoginAsGuestUser,
        ExecuteAction = async (parameter) => await LoginAsGuestUser()
      };
    }

    ///<inheritdoc/>
    public async override Task<bool> InitializeAsync()
    {
      User = new User();
      claimsPrincipal = (await AuthenticationStateTask).User;
      if (claimsPrincipal.Identity.IsAuthenticated)
        return false;
      return true;
    }

    /// <summary>
    /// Tries to log in
    /// </summary>
    /// <returns>Task with result</returns>
    protected override async Task SubmitAsyncCore()
    {
      ApiReply<User> reply = await ApiConnector.PostAsync<User>("Users/Login", User);
      if (reply.WasSuccessful)
      {
        await AuthenticationStateProvider.MarkUserAsAuthenticated(reply.Result);
        NavigationManager.NavigateTo("/");
      }
      else
        ErrorMessage = Errors.InvalidUserNameOrPassword;
    }

    private async Task LoginAsGuestUser()
    {
      User = User.GuestUser;
      await SubmitAsync();
    }
  }
}