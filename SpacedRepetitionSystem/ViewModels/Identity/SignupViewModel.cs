using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using SpacedRepetitionSystem.Components.Middleware;
using SpacedRepetitionSystem.Entities;
using SpacedRepetitionSystem.Entities.Entities.Security;
using SpacedRepetitionSystem.Entities.Validation.Core;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.ViewModels.Identity
{
  /// <summary>
  /// ViewModel for a signup page
  /// </summary>
  public sealed class SignupViewModel : IdentityViewModelBase
  {
    /// <summary>
    /// The Confirm password
    /// </summary>
    public string ConfirmPassword { get; set; }

    /// <summary>
    /// Whether confirm password is invalid
    /// </summary>
    public bool HasConfirmPasswordError { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="navigationManager">NavigationManager (Injected)</param>
    /// <param name="authenticationStateProvider">AuthenticationStateProvider (Injected)</param>
    /// <param name="apiConnector">API Connector (Injected)</param>
    /// <param name="changeValidator">EntityChangeValdiator (Injected)</param>

    public SignupViewModel(NavigationManager navigationManager, AuthenticationStateProvider authenticationStateProvider, 
      IApiConnector apiConnector, EntityChangeValidator<User> changeValidator) 
      : base(navigationManager, authenticationStateProvider, apiConnector, changeValidator)
    { }

    /// <summary>
    /// Tries to sign up
    /// </summary>
    protected override async Task SubmitAsyncCore()
    {
      ApiReply<User> reply = await ApiConnector.PostAsync<User>("Users/Signup", User);
      if (reply.WasSuccessful)
      {
        await AuthenticationStateProvider.MarkUserAsAuthenticated(reply.Result);
        NavigationManager.NavigateTo("/");
      }
      else
        ErrorMessage = reply.ResultMessage;
    }

    ///<inheritdoc/>
    protected override bool ValidateUser()
    {
      HasConfirmPasswordError = false;
      bool result = base.ValidateUser();
      if (!result)
        return false;

      if (User.Password != ConfirmPassword)
      {
        ErrorMessage = Errors.PasswordDoesNotEqualConfirm;
        HasPasswordError = HasConfirmPasswordError = true;
        return false;
      }
      return true;
    }
  }
}