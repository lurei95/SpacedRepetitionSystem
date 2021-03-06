﻿using Microsoft.AspNetCore.Components;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using SpacedRepetitionSystem.Components.Pages;
using SpacedRepetitionSystem.Components.ViewModels;
using System;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Components.Tests.Pages
{
  /// <summary>
  /// Testclass for <see cref="PageBase"/>
  /// </summary>
  [TestClass]
  public sealed class PageBaseTests
  {
    public sealed class TestViewModel : PageViewModelBase
    {
      public bool InitializeResult { get; set; }

      public bool WasInitializeCalled { get; set; }

      public TestViewModel(NavigationManager navigationManager) : base(navigationManager)
      { }

      public override string Title => throw new NotImplementedException();

      public override Task<bool> InitializeAsync()
      {
        WasInitializeCalled = true;
        return Task.FromResult(InitializeResult);
      }
    }

    public class TestPage : PageBase<TestViewModel>
    {
      public async Task CallOnAfterRenderAsync(bool firstRender)
      { await base.OnAfterRenderAsync(firstRender); }
    }

    /// <summary>
    /// Tests that <see cref="PageViewModelBase.InitializeAsync"/> is called when the page is loaded
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task InitializesViewModelOnLoadSuccessTest()
    {
      Mock<TestPage> mockPage = new Mock<TestPage>();
      mockPage.Protected().Setup("NotifyStateChanged").Callback(() => { });
      NavigationManagerMock navigationManagerMock = new NavigationManagerMock();
      TestViewModel viewModel = new TestViewModel(navigationManagerMock);
      TestPage page = mockPage.Object;
      page.ViewModel = viewModel;
      page.NavigationManager = navigationManagerMock;
      viewModel.InitializeResult = true;

      await page.CallOnAfterRenderAsync(false);
      Assert.IsFalse(viewModel.WasInitializeCalled);
      Assert.IsTrue(page.IsLoading);

      await page.CallOnAfterRenderAsync(true);
      Assert.IsTrue(viewModel.WasInitializeCalled);
      Assert.IsFalse(page.IsLoading);
    }

    /// <summary>
    /// Tests that <see cref="PageViewModelBase.InitializeAsync"/> is called when the page is loaded
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task InitializesViewModelOnLoadFailedTest()
    {
      NavigationManagerMock navigationManagerMock = new NavigationManagerMock();
      TestViewModel viewModel = new TestViewModel(navigationManagerMock);
      TestPage page = new TestPage
      {
        ViewModel = viewModel,
        NavigationManager = navigationManagerMock,
      };
      viewModel.InitializeResult = false;

      await page.CallOnAfterRenderAsync(false);
      Assert.IsFalse(viewModel.WasInitializeCalled);
      Assert.IsTrue(page.IsLoading);

      await page.CallOnAfterRenderAsync(true);
      Assert.AreEqual(navigationManagerMock.NavigatedUri, "/");
      Assert.IsTrue(viewModel.WasInitializeCalled);
    }
  }
}
