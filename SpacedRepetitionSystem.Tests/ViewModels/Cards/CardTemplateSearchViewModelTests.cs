using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpacedRepetitionSystem.Components.Middleware;
using SpacedRepetitionSystem.Components.Tests;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Utility.Notification;
using SpacedRepetitionSystem.Utility.Tests.Notification;
using SpacedRepetitionSystem.ViewModels.Cards;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Tests.ViewModels.Cards
{
  /// <summary>
  /// Testclass for <see cref="CardTemplateSearchViewModel"/>
  /// </summary>
  [TestClass]
  public sealed class CardTemplateSearchViewModelTests
  {
    private readonly NavigationManagerMock navigationManagerMock = new NavigationManagerMock();
    private readonly NotificationProviderMock notificationProviderMock = new NotificationProviderMock();

    /// <summary>
    /// Initializes the tests
    /// </summary>
    [TestInitialize]
    public void TestInitialize()
    { NotificationMessageProvider.Initialize(notificationProviderMock, 500000); }

    /// <summary>
    /// Tests that the search was executed on initialize
    /// </summary>
    [TestMethod]
    public async Task DoesExecuteSearchOnInitializeTest()
    {
      CardTemplate template = new CardTemplate();
      ApiConnectorMock mock = CreateMockForInitialize(new List<CardTemplate>() { template });
      NavigationManagerMock navigationManagerMock = new NavigationManagerMock();
      CardTemplateSearchViewModel viewModel = new CardTemplateSearchViewModel(navigationManagerMock, mock);
      bool result = await viewModel.InitializeAsync();

      Assert.IsTrue(result);
      Assert.AreEqual(1, viewModel.SearchResults.Count);
      Assert.AreSame(template, viewModel.SearchResults[0]);
    }

    /// <summary>
    /// Tests that the search parameters are send correctly when the search is executed
    /// </summary>
    [TestMethod]
    public async Task ExecuteSearchParametersTest()
    {
      ApiConnectorMock mock = CreateMockForInitialize(new List<CardTemplate>());
      CardTemplateSearchViewModel viewModel = new CardTemplateSearchViewModel(navigationManagerMock, mock);

      await viewModel.SearchAsync();
      Dictionary<string, object> parameters = mock.Parameter as Dictionary<string, object>;
      Assert.AreEqual(0, parameters.Count);

      viewModel.SearchText = "text";
      mock.Replies.Push(new ApiReply<List<CardTemplate>>()
      {
        WasSuccessful = true,
        Result = new List<CardTemplate>()
      });
      await viewModel.SearchAsync();
      parameters = mock.Parameter as Dictionary<string, object>;
      Assert.AreEqual(1, parameters.Count);
      Assert.AreEqual("text", parameters[nameof(viewModel.SearchText)]);
    }

    /// <summary>
    /// Tests that all commands are initialized correctly
    /// </summary>
    [TestMethod]
    public async Task CommandsAreInitializedCorrectlyTest()
    {
      CardTemplate template = new CardTemplate() { CardTemplateId = 2 };
      ApiConnectorMock mock = CreateMockForInitialize(new List<CardTemplate>());
      CardTemplateSearchViewModel viewModel = new CardTemplateSearchViewModel(navigationManagerMock, mock)
      { SelectedEntity = template };
      await viewModel.InitializeAsync();

      Assert.IsNotNull(viewModel.NewCommand.CommandText);
      Assert.IsNotNull(viewModel.NewCommand.ToolTip);
      Assert.AreEqual("/New", viewModel.NewCommand.TargetUri);
      Assert.IsTrue(viewModel.NewCommand.IsRelative);

      Assert.IsNotNull(viewModel.EditCommand.CommandText);
      Assert.IsNotNull(viewModel.EditCommand.ToolTip);
      Assert.AreEqual("/2", viewModel.EditCommand.TargetUriFactory.Invoke(template));
      Assert.IsTrue(viewModel.EditCommand.IsRelative);

      Assert.IsNotNull(viewModel.DeleteCommand.CommandText);
      Assert.IsNotNull(viewModel.DeleteCommand.ToolTip);
    }

    private ApiConnectorMock CreateMockForInitialize(List<CardTemplate> templates)
    {
      ApiConnectorMock mock = new ApiConnectorMock();
      mock.Replies.Push(new ApiReply<List<CardTemplate>>()
      {
        WasSuccessful = true,
        Result = templates
      });
      return mock;
    }
  }
}
