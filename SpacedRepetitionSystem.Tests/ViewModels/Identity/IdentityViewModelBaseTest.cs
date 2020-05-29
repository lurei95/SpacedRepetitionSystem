using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpacedRepetitionSystem.Components.Middleware;
using SpacedRepetitionSystem.Components.Tests;
using SpacedRepetitionSystem.Entities.Entities.Security;
using SpacedRepetitionSystem.Entities.Validation.Core;
using SpacedRepetitionSystem.Entities.Validation.Users;
using SpacedRepetitionSystem.ViewModels.Identity;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Tests.ViewModels.Identity
{
  /// <summary>
  /// Testclass for <see cref="IdentityViewModelBase"/>
  /// </summary>
  [TestClass]
  public sealed class IdentityViewModelBaseTest
  {
    private static readonly NavigationManagerMock navigationManagerMock = new NavigationManagerMock();

    private class TestViewModel : IdentityViewModelBase
    {
      public bool WasSubmitted { get; set; }

      public TestViewModel(NavigationManager navigationManager, AuthenticationStateProvider authenticationStateProvider, 
        IApiConnector apiConnector, EntityChangeValidator<User> changeValidator) 
        : base(navigationManager, authenticationStateProvider, apiConnector, changeValidator)
      { }

      protected override Task SubmitAsyncCore()
      {
        WasSubmitted = true;
        Assert.IsTrue(IsBusy);
        return Task.FromResult<object>(null);
      }

      public void ExecuteValidateUser()
      { ValidateUser(); }
    }

    /// <summary>
    /// Tests <see cref="IdentityViewModelBase.SubmitAsync"/>
    /// </summary>
    [TestMethod]
    public async Task SubmitAsyncTest()
    {
      ApiConnectorMock mock = new ApiConnectorMock();
      LocalStorageServiceMock localStorageServiceMock = new LocalStorageServiceMock();
      CustomAuthenticationStateProvider authenticationStateProvider 
        = new CustomAuthenticationStateProvider(localStorageServiceMock, mock, navigationManagerMock);
      TestViewModel viewModel = new TestViewModel(navigationManagerMock, authenticationStateProvider, mock, new EntityChangeValidator<User>());
      await viewModel.SubmitAsync();

      Assert.IsTrue(viewModel.WasSubmitted);
      Assert.IsFalse(viewModel.IsBusy);
    }

    /// <summary>
    /// Tests <see cref="IdentityViewModelBase.SubmitAsync"/>
    /// </summary>
    [TestMethod]
    public async Task DoesNotSubmitIfValidationReturnsErrorTest()
    {
      User user = new User();
      ApiConnectorMock mock = new ApiConnectorMock();
      EntityChangeValidator<User> validator = new EntityChangeValidator<User>();
      validator.Register(nameof(User.Email), new UserEmailValidator());
      LocalStorageServiceMock localStorageServiceMock = new LocalStorageServiceMock();
      CustomAuthenticationStateProvider authenticationStateProvider
        = new CustomAuthenticationStateProvider(localStorageServiceMock, mock, navigationManagerMock);
      TestViewModel viewModel = new TestViewModel(navigationManagerMock, authenticationStateProvider, mock, validator)
      { User = user };
      await viewModel.SubmitAsync();

      Assert.IsFalse(viewModel.WasSubmitted);
      Assert.IsFalse(viewModel.IsBusy);
    }

    /// <summary>
    /// Tests <see cref="IdentityViewModelBase.ValidateUser"/>
    /// </summary>
    [TestMethod]
    public void ValidateUserTest()
    {
      User user = new User();
      ApiConnectorMock mock = new ApiConnectorMock();
      LocalStorageServiceMock localStorageServiceMock = new LocalStorageServiceMock();
      CustomAuthenticationStateProvider authenticationStateProvider
        = new CustomAuthenticationStateProvider(localStorageServiceMock, mock, navigationManagerMock);
      EntityChangeValidator<User> validator = new EntityChangeValidator<User>();
      validator.Register(nameof(user.Email), new UserEmailValidator());
      validator.Register(nameof(user.Password), new UserPasswordValidator());
      TestViewModel viewModel = new TestViewModel(navigationManagerMock, authenticationStateProvider, mock, validator)
      { User = user };

      //Validates empty email
      user.Password = "test";
      viewModel.ExecuteValidateUser();
      Assert.IsTrue(viewModel.HasEmailError);
      Assert.IsFalse(viewModel.HasPasswordError);
      Assert.IsFalse(string.IsNullOrEmpty(viewModel.ErrorMessage));

      //Validates empty password
      user.Email = "test";
      user.Password = null;
      viewModel.ExecuteValidateUser();
      Assert.IsFalse(viewModel.HasEmailError);
      Assert.IsTrue(viewModel.HasPasswordError);
      Assert.IsFalse(string.IsNullOrEmpty(viewModel.ErrorMessage));

      //No error
      user.Email = "test";
      user.Password = "test";
      viewModel.ExecuteValidateUser();
      Assert.IsFalse(viewModel.HasEmailError);
      Assert.IsFalse(viewModel.HasPasswordError);
      Assert.IsTrue(string.IsNullOrEmpty(viewModel.ErrorMessage));
    }
  }
}