using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpacedRepetitionSystem.Components.Middleware;
using SpacedRepetitionSystem.Components.Tests;
using SpacedRepetitionSystem.Entities;
using SpacedRepetitionSystem.Entities.Entities.Security;
using SpacedRepetitionSystem.Entities.Validation.Core;
using SpacedRepetitionSystem.ViewModels.Identity;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Tests.ViewModels.Identity
{
  /// <summary>
  /// Testclass for <see cref="LoginViewModel"/>
  /// </summary>
  [TestClass]
  public sealed class LoginViewModelTests
  {
    private static readonly NavigationManagerMock navigationManagerMock = new NavigationManagerMock();

    /// <summary>
    /// Tests <see cref="LoginViewModel"/>
    /// </summary>
    [TestMethod]
    public async Task InitializeAsyncTest()
    {
      ApiConnectorMock mock = new ApiConnectorMock();
      LocalStorageServiceMock localStorageServiceMock = new LocalStorageServiceMock();
      CustomAuthenticationStateProvider authenticationStateProvider
        = new CustomAuthenticationStateProvider(localStorageServiceMock, mock, navigationManagerMock);

      //Not Authenticated
      LoginViewModel viewModel = new LoginViewModel(navigationManagerMock, authenticationStateProvider, mock, new EntityChangeValidator<User>())
      { AuthenticationStateTask = Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()))) };
      bool result = await viewModel.InitializeAsync();
      Assert.IsTrue(result);
      Assert.IsNotNull(viewModel.User);

      //Authenticated
      ClaimsIdentity identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "test") }, "apiauth_type");
      viewModel.AuthenticationStateTask = Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity)));
      result = await viewModel.InitializeAsync();
      Assert.IsFalse(result);
    }

    /// <summary>
    /// Tests thta the commands are initialized correctly
    /// </summary>
    [TestMethod]
    public void CommandsAreInitializedCorrectlyTest()
    {
      CustomAuthenticationStateProvider authenticationStateProvider
        = new CustomAuthenticationStateProvider(new LocalStorageServiceMock(), new ApiConnectorMock(), navigationManagerMock);
      LoginViewModel viewModel = new LoginViewModel(navigationManagerMock, authenticationStateProvider, 
        new ApiConnectorMock(), new EntityChangeValidator<User>());

      Assert.IsFalse(string.IsNullOrEmpty(viewModel.LoginAsGuestUserCommand.CommandText));
      Assert.IsFalse(string.IsNullOrEmpty(viewModel.LoginAsGuestUserCommand.ToolTip));
      Assert.IsTrue(viewModel.LoginAsGuestUserCommand.IsEnabled);
    }

    /// <summary>
    /// Tests <see cref="LoginViewModel.SubmitAsyncCore"/> when signup is successful
    /// </summary>
    [TestMethod]
    public async Task LoginSuccessfullyTest()
    { await LoginTestCore(true); }

    /// <summary>
    /// Tests <see cref="LoginViewModel.SubmitAsyncCore"/> when signup returns an error
    /// </summary>
    [TestMethod]
    public async Task LoginErrorTest()
    { await LoginTestCore(false); }

    /// <summary>
    /// Tests <see cref="LoginViewModel.LoginAsGuestUserCommand"/>
    /// </summary>
    [TestMethod]
    public void LoginAsGuestUserCommandTest()
    {
      User user = new User() { Email = "test@test.com", Password = "test" };
      ApiConnectorMock mock = new ApiConnectorMock();
      mock.Replies.Push(new ApiReply<User>()
      {
        WasSuccessful = true,
        Result = User.GuestUser,
      });
      LocalStorageServiceMock localStorageServiceMock = new LocalStorageServiceMock();
      CustomAuthenticationStateProvider authenticationStateProvider
        = new CustomAuthenticationStateProvider(localStorageServiceMock, mock, navigationManagerMock);
      LoginViewModel viewModel = new LoginViewModel(navigationManagerMock, authenticationStateProvider, mock, new EntityChangeValidator<User>())
      { User = user };
      viewModel.LoginAsGuestUserCommand.ExecuteCommand();

      Assert.AreSame(User.GuestUser, mock.Parameters.Pop());
      Assert.AreEqual("Users/Login", mock.Routes.Pop());
      Assert.AreEqual(HttpMethod.Post, mock.Methods.Pop());
    }

    private async Task LoginTestCore(bool successful)
    {
      User user = new User() { Email = "test@test.com", Password = "test" };
      ApiConnectorMock mock = new ApiConnectorMock();
      mock.Replies.Push(new ApiReply<User>()
      {
        WasSuccessful = successful,
        Result = successful ? user : null,
      });
      LocalStorageServiceMock localStorageServiceMock = new LocalStorageServiceMock();
      CustomAuthenticationStateProvider authenticationStateProvider
        = new CustomAuthenticationStateProvider(localStorageServiceMock, mock, navigationManagerMock);
      LoginViewModel viewModel = new LoginViewModel(navigationManagerMock, authenticationStateProvider, mock, new EntityChangeValidator<User>())
      { User = user };
      await viewModel.SubmitAsync();

      Assert.AreSame(user, mock.Parameters.Pop());
      Assert.AreEqual("Users/Login", mock.Routes.Pop());
      Assert.AreEqual(HttpMethod.Post, mock.Methods.Pop());

      if (successful)
      {
        Assert.AreSame(user, mock.CurrentUser);
        Assert.AreSame("/", navigationManagerMock.NavigatedUri);
      }
      else
        Assert.AreEqual(Errors.InvalidUserNameOrPassword, viewModel.ErrorMessage);
    }
  }
}