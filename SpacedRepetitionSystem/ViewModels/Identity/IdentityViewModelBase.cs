using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using SpacedRepetitionSystem.Components.Middleware;
using SpacedRepetitionSystem.Components.ViewModels;
using SpacedRepetitionSystem.Entities.Entities.Security;
using SpacedRepetitionSystem.Entities.Validation.Core;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.ViewModels.Identity
{
  /// <summary>
  /// Base class for login or signup ViewModel
  /// </summary>
  public abstract class IdentityViewModelBase : PageViewModelBase
  {
    private readonly EntityChangeValidator<User> changeValidator;

    /// <summary>
    /// The api connetcor
    /// </summary>
    protected IApiConnector ApiConnector { get; private set; }

    /// <summary>
    /// Whether the e-mail is invalid
    /// </summary>
    public bool HasEmailError { get; set; }

    /// <summary>
    /// Whether the password is invalid
    /// </summary>
    public bool HasPasswordError { get; set; }

    /// <summary>
    /// The authemntication state provider
    /// </summary>
    protected CustomAuthenticationStateProvider AuthenticationStateProvider { get; private set; }

    /// <summary>
    /// The error message
    /// </summary>
    public string ErrorMessage { get; set; }

    /// <summary>
    /// Whether the ViewModel is currently logging in
    /// </summary>
    public bool IsBusy { get; set; }

    /// <summary>
    /// The user
    /// </summary>
    public User User { get; set; } = new User();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="navigationManager">NavigationManager (Injected)</param>
    /// <param name="authenticationStateProvider">AuthenticationStateProvider (Injected)</param>
    /// <param name="apiConnector">API Connector (Injected)</param>
    /// <param name="changeValidator">EntityChangeValdiator (Injected)</param>
    public IdentityViewModelBase(NavigationManager navigationManager, AuthenticationStateProvider authenticationStateProvider, 
      IApiConnector apiConnector, EntityChangeValidator<User> changeValidator)
      : base(navigationManager)
    {
      this.changeValidator = changeValidator;
      ApiConnector = apiConnector;
      AuthenticationStateProvider = authenticationStateProvider as CustomAuthenticationStateProvider;
    }

    /// <summary>
    /// Submit the result
    /// </summary>
    /// <returns></returns>
    public virtual async Task SubmitAsync()
    {
      IsBusy = true;
      if (ValidateUser())
        await SubmitAsyncCore();
      IsBusy = false;
    }

    /// <summary>
    /// Action performaed on submit
    /// </summary>
    /// <returns></returns>
    protected abstract Task SubmitAsyncCore();

    /// <summary>
    /// Valdiates the user befor submitting
    /// </summary>
    protected virtual bool ValidateUser()
    {
      ErrorMessage = null;
      HasEmailError = HasPasswordError = false;

      string error = changeValidator.Validate(nameof(User.UserId), User, User.UserId);
      if (!string.IsNullOrEmpty(error))
      {
        ErrorMessage = error;
        HasEmailError = true;
        return false;
      }

      error = changeValidator.Validate(nameof(User.Password), User, User.Password);
      if (!string.IsNullOrEmpty(error))
      {
        ErrorMessage = error;
        HasPasswordError = true;
        return false;
      }
      return true;
    }
  }
}