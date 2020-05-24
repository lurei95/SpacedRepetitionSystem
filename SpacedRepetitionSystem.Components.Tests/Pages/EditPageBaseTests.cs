using Microsoft.AspNetCore.Components;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpacedRepetitionSystem.Components.Middleware;
using SpacedRepetitionSystem.Components.Pages;
using SpacedRepetitionSystem.Components.ViewModels;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Entities.Validation.Core;
using System;

namespace SpacedRepetitionSystem.Components.Tests.Pages
{
  /// <summary>
  /// Testclass for <see cref="EditPageBase{TEntity, TViewModel}"/>
  /// </summary>
  [TestClass]
  public sealed class EditPageBaseTests
  {
    private sealed class TestViewModel : EditViewModelBase<Card>
    {
      public TestViewModel(NavigationManager navigationManager, IApiConnector apiConnector, EntityChangeValidator<Card> changeValidator) 
        : base(navigationManager, apiConnector, changeValidator)
      {
      }

      public bool InitializeResult { get; set; }

      public bool WasInitializeCalled { get; set; }

      public override string Title => throw new NotImplementedException();

      protected override void CreateNewEntity()
      {
        throw new NotImplementedException();
      }
    }

    private sealed class TestPage : EditPageBase<Card, TestViewModel>
    { }

    /// <summary>
    /// Tests that <see cref="EditPageBase{TEntity, TViewModel}.Id"/> is called when the page is loaded
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "BL0005:Component parameter should not be set outside of its component.", Justification = "<Pending>")]
    public void SettingIdSetsViewModelsIdTest()
    {
      NavigationManagerMock navigationManagerMock = new NavigationManagerMock();
      TestViewModel viewModel = new TestViewModel(navigationManagerMock, new ApiConnectorMock(), new EntityChangeValidator<Card>());
      TestPage page = new TestPage
      {
        ViewModel = viewModel,
        NavigationManager = navigationManagerMock,
      };
      page.Id = 3;
      Assert.AreEqual(3, viewModel.Id);
    }
  }
}