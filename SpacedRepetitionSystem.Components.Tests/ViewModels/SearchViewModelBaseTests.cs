using Microsoft.AspNetCore.Components;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpacedRepetitionSystem.Components.Middleware;
using SpacedRepetitionSystem.Components.ViewModels;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Components.Tests.ViewModels
{
  /// <summary>
  /// Testclass for <see cref="SearchViewModelBaseTests{TEntity}"/>
  /// </summary>
  [TestClass]
  public sealed class SearchViewModelBaseTests
  {
    private static readonly NavigationManagerMock navigationManagerMock = new NavigationManagerMock();
    private static readonly ApiConnectorMock apiConnectorMock = new ApiConnectorMock();
    private static readonly List<Card> results = new List<Card>() 
    { 
      new Card() { CardId = 1 },
      new Card() { CardId = 2 }
    };

    private sealed class TestViewModel : SearchViewModelBase<Card>
    {
      public List<Card> Results { get; set; }

      public bool SearchExecuted { get; set; }

      public TestViewModel(NavigationManager navigationManager, IApiConnector apiConnector) 
        : base(navigationManager, apiConnector)
      { }

      protected override async Task<List<Card>> SearchCore()
      {
        SearchExecuted = true;
        return await Task.FromResult(Results);
      }
    }

    /// <summary>
    /// Tests <see cref="SearchViewModelBase{TEntity}.SearchAsync"/>
    /// </summary>
    [TestMethod]
    public async Task SearchAsyncTest()
    {
      TestViewModel viewModel = new TestViewModel(navigationManagerMock, apiConnectorMock)
      { Results = results };
      await viewModel.SearchAsync();
      Assert.IsTrue(viewModel.SearchExecuted);
      Assert.AreEqual(2, viewModel.SearchResults.Count);
      Assert.AreSame(results[0], viewModel.SearchResults[0]);
      Assert.AreSame(results[1], viewModel.SearchResults[1]);
      Assert.AreSame(results[0], viewModel.SelectedEntity);
    }
  }
}