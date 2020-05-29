using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpacedRepetitionSystem.Components.Middleware;
using SpacedRepetitionSystem.Components.Tests;
using SpacedRepetitionSystem.Entities.Entities.Security;
using SpacedRepetitionSystem.Entities.Validation.Core;
using SpacedRepetitionSystem.ViewModels.Identity;
using System.Net.Http;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Tests.ViewModels.Identity
{
  /// <summary>
  /// Testclass for <see cref="SignupViewModel"/>
  /// </summary>
  [TestClass]
  public sealed class SignupViewModelTests
  {
    private static readonly NavigationManagerMock navigationManagerMock = new NavigationManagerMock();

    /// <summary>
    /// Tests <see cref="SignupViewModel.SubmitAsyncCore"/> when signup is successful
    /// </summary>
    [TestMethod]
    public async Task SignupSuccessfullyTest()
    { await SignupTestCore(true); }

    /// <summary>
    /// Tests <see cref="SignupViewModel.SubmitAsyncCore"/> when signup returns an error
    /// </summary>
    [TestMethod]
    public async Task SignupErrorTest()
    { await SignupTestCore(false); }

    /// <summary>
    /// Tests that <see cref="SignupViewModel.ConfirmPassword"/> is validated
    /// </summary>
    [TestMethod]
    public async Task ValidatesConfirmPasswordTest()
    {
      User user = new User() { Email = "test@test.com", Password = "test" };
      ApiConnectorMock mock = new ApiConnectorMock();
      LocalStorageServiceMock localStorageServiceMock = new LocalStorageServiceMock();
      CustomAuthenticationStateProvider authenticationStateProvider
        = new CustomAuthenticationStateProvider(localStorageServiceMock, mock, navigationManagerMock);
      SignupViewModel viewModel = new SignupViewModel(navigationManagerMock, authenticationStateProvider, mock, new EntityChangeValidator<User>())
      { 
        User = user, 
        ConfirmPassword = "test124"
      };

      await viewModel.SubmitAsync();
      Assert.IsTrue(viewModel.HasConfirmPasswordError);
      Assert.IsFalse(string.IsNullOrEmpty(viewModel.ErrorMessage));

      viewModel.ConfirmPassword = "test";
      mock.Replies.Push(new ApiReply<User>()
      {
        WasSuccessful = true,
        Result = user
      });
      await viewModel.SubmitAsync();
      Assert.IsFalse(viewModel.HasConfirmPasswordError);
      Assert.IsTrue(string.IsNullOrEmpty(viewModel.ErrorMessage));
    }

    private async Task SignupTestCore(bool successful)
    {
      User user = new User() { Email = "test@test.com", Password = "test" };
      ApiConnectorMock mock = new ApiConnectorMock();
      mock.Replies.Push(new ApiReply<User>() 
      { 
        WasSuccessful = successful, 
        Result = successful ? user : null,
        ResultMessage = successful ? null : "test-error"
      });
      LocalStorageServiceMock localStorageServiceMock = new LocalStorageServiceMock();
      CustomAuthenticationStateProvider authenticationStateProvider
        = new CustomAuthenticationStateProvider(localStorageServiceMock, mock, navigationManagerMock);
      SignupViewModel viewModel = new SignupViewModel(navigationManagerMock, authenticationStateProvider, mock, new EntityChangeValidator<User>())
      { User = user };
      viewModel.ConfirmPassword = "test";
      await viewModel.SubmitAsync();

      Assert.AreSame(user, mock.Parameter);
      Assert.AreEqual("Users/Signup", mock.Route);
      Assert.AreEqual(HttpMethod.Post, mock.Method);

      if (successful)
      {
        Assert.AreSame(user, mock.CurrentUser);
        Assert.AreSame("/", navigationManagerMock.NavigatedUri);
      }
      else
        Assert.AreEqual("test-error", viewModel.ErrorMessage);
    }
  }
}