using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using SpacedRepetitionSystem.Entities;
using SpacedRepetitionSystem.Entities.Entities.Users;
using SpacedRepetitionSystem.Entities.Validation.Core;
using SpacedRepetitionSystem.Logic.Controllers.Core;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Components.ViewModels.Identity
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
      if (ApiConnector.Post(User))
      {
        await AuthenticationStateProvider.MarkUserAsAuthenticated(User);
        NavigationManager.NavigateTo("/");
      }
      else
        ErrorMessage = Errors.UserAlreadyExists;
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