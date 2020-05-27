using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpacedRepetitionSystem.Components.Middleware;
using SpacedRepetitionSystem.Components.Tests;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Entities.Validation.Core;
using SpacedRepetitionSystem.Entities.Validation.Decks;
using SpacedRepetitionSystem.Utility.Notification;
using SpacedRepetitionSystem.Utility.Tests.Notification;
using SpacedRepetitionSystem.ViewModels.Cards;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Tests.ViewModels.Cards
{
  /// <summary>
  /// Testclass for <see cref="DeckEditViewModel"/>
  /// </summary>
  [TestClass]
  public sealed class DeckEditViewModelTests
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
    /// Tests that the available templatzes are loaded when the view model is initialized
    /// </summary>
    [TestMethod]
    public async Task LoadsAvailableCardTemplatesOnInitializeTest()
    {
      CardTemplate template = new CardTemplate() { CardTemplateId = 1, Title = "test" };
      ApiConnectorMock mock = CreateMockForInitialize(false, true, null, new List<CardTemplate>() { template });
      DeckEditViewModel viewModel = new DeckEditViewModel(navigationManagerMock, mock, new EntityChangeValidator<Deck>());
      bool result = await viewModel.InitializeAsync();

      Assert.IsTrue(result);
      Assert.AreEqual(1, viewModel.AvailableCardTemplates.Count);
      Assert.AreEqual("test", viewModel.AvailableCardTemplates[0]);
    }

    /// <summary>
    /// Tests the behavior when an error is returned while loading the available templatzes when the view model is initialized
    /// </summary>
    [TestMethod]
    public async Task LoadsAvailableCardTemplatesOnInitializeErrorTest()
    {
      CardTemplate template = new CardTemplate() { CardTemplateId = 1, Title = "test" };
      ApiConnectorMock mock = CreateMockForInitialize(false, false, null, new List<CardTemplate>() { template });
      DeckEditViewModel viewModel = new DeckEditViewModel(navigationManagerMock, mock, new EntityChangeValidator<Deck>());
      bool result = await viewModel.InitializeAsync();

      Assert.IsFalse(result);
      Assert.AreEqual(0, viewModel.AvailableCardTemplates.Count);
      Assert.AreEqual(NotificationKind.ErrorNotification, notificationProviderMock.NotificationKind);
      Assert.AreEqual("test-error", notificationProviderMock.Message);
    }

    /// <summary>
    /// Tests the creation of a new deck on <see cref="DeckEditViewModel.InitializeAsync"/>
    /// </summary>
    [TestMethod]
    public async Task CreateNewDeckOnInitializeTest()
    {
      CardTemplate template = new CardTemplate() { CardTemplateId = 1, Title = "test" };
      ApiConnectorMock mock = CreateMockForInitialize(false, true, null, new List<CardTemplate>() { template });
      DeckEditViewModel viewModel = new DeckEditViewModel(navigationManagerMock, mock, new EntityChangeValidator<Deck>());
      bool result = await viewModel.InitializeAsync();

      Assert.IsTrue(result);
      Assert.IsNotNull(viewModel.Entity);
      //Load default template
      Assert.AreEqual(1, viewModel.CardTemplateId);
      Assert.AreEqual(1, viewModel.Entity.DefaultCardTemplateId);
      Assert.AreEqual("test", viewModel.CardTemplateTitle);
    }

    /// <summary>
    /// Tests that all commands are initialized correctly
    /// </summary>
    [TestMethod]
    public async Task CommandsAreInitializedCorrectlyTest()
    {
      Deck deck = new Deck() { DeckId = 1, Title = "test", DefaultCardTemplateId = 1 };
      deck.Cards.Add(new Card() { CardId = 1 });
      CardTemplate template = new CardTemplate() { CardTemplateId = 1, Title = "test" };
      ApiConnectorMock mock = CreateMockForInitialize(true, true, deck, new List<CardTemplate>() { template });
      DeckEditViewModel viewModel = new DeckEditViewModel(navigationManagerMock, mock, new EntityChangeValidator<Deck>());
      await viewModel.InitializeAsync();

      Assert.IsNotNull(viewModel.ShowStatisticsCommand.CommandText);
      Assert.IsNotNull(viewModel.ShowStatisticsCommand.ToolTip);
      Assert.AreEqual("/Statistics", viewModel.ShowStatisticsCommand.TargetUri);
      Assert.IsTrue(viewModel.ShowStatisticsCommand.IsRelative);

      Assert.IsNotNull(viewModel.EditCardCommand.CommandText);
      Assert.IsNotNull(viewModel.EditCardCommand.ToolTip);
      Assert.AreEqual("/Cards/1", viewModel.EditCardCommand.TargetUriFactory.Invoke(deck.Cards[0]));
      Assert.IsTrue(viewModel.EditCardCommand.IsRelative);

      Assert.IsNotNull(viewModel.NewCardCommand.CommandText);
      Assert.IsNotNull(viewModel.NewCardCommand.ToolTip);
      Assert.AreEqual("/Cards/New", viewModel.NewCardCommand.TargetUri);
      Assert.IsTrue(viewModel.NewCardCommand.IsRelative);

      Assert.IsNotNull(viewModel.PracticeDeckCommand.CommandText);
      Assert.IsNotNull(viewModel.PracticeDeckCommand.ToolTip);
      Assert.AreEqual("/Practice", viewModel.PracticeDeckCommand.TargetUri);
      Assert.IsTrue(viewModel.PracticeDeckCommand.IsRelative);

      Assert.IsNotNull(viewModel.DeleteCommand.CommandText);
      Assert.IsNotNull(viewModel.DeleteCommand.ToolTip);

      Assert.IsNotNull(viewModel.DeleteCardCommand.CommandText);
      Assert.IsNotNull(viewModel.DeleteCardCommand.ToolTip);
    }

    /// <summary>
    /// Tests the enabled bahavior of the commands
    /// </summary>
    [TestMethod]
    public async Task CommandsEnabledTest()
    {
      CardTemplate template = new CardTemplate() { CardTemplateId = 1, Title = "test" };

      //New Entity
      ApiConnectorMock mock = CreateMockForInitialize(false, true, null, new List<CardTemplate>() { template });
      DeckEditViewModel viewModel = new DeckEditViewModel(navigationManagerMock, mock, new EntityChangeValidator<Deck>());
      await viewModel.InitializeAsync();
      Assert.IsFalse(viewModel.PracticeDeckCommand.IsEnabled);
      Assert.IsFalse(viewModel.ShowStatisticsCommand.IsEnabled);
      Assert.IsFalse(viewModel.DeleteCommand.IsEnabled);
      Assert.IsFalse(viewModel.EditCardCommand.IsEnabled);
      Assert.IsFalse(viewModel.DeleteCardCommand.IsEnabled);
      Assert.IsFalse(viewModel.NewCardCommand.IsEnabled);
      Assert.IsTrue(viewModel.SaveChangesCommand.IsEnabled);
      Assert.IsTrue(viewModel.CloseCommand.IsEnabled);

      //Existing entity
      Deck deck = new Deck() { DeckId = 1, Title = "test", DefaultCardTemplateId = 1 };
      mock = CreateMockForInitialize(true, true, deck, new List<CardTemplate>() { template });
      viewModel = new DeckEditViewModel(navigationManagerMock, mock, new EntityChangeValidator<Deck>())
      { Id = 1 };
      await viewModel.InitializeAsync();
      Assert.IsTrue(viewModel.PracticeDeckCommand.IsEnabled);
      Assert.IsTrue(viewModel.ShowStatisticsCommand.IsEnabled);
      Assert.IsTrue(viewModel.DeleteCommand.IsEnabled);
      Assert.IsTrue(viewModel.EditCardCommand.IsEnabled);
      Assert.IsTrue(viewModel.DeleteCardCommand.IsEnabled);
      Assert.IsTrue(viewModel.NewCardCommand.IsEnabled);
      Assert.IsTrue(viewModel.SaveChangesCommand.IsEnabled);
      Assert.IsTrue(viewModel.CloseCommand.IsEnabled);
    }

    /// <summary>
    /// Tests <see cref="DeckEditViewModel.TitleProperty"/> is validated
    /// </summary>
    [TestMethod]
    public async Task DoesValidateTitlePropertyTest()
    {
      EntityChangeValidator<Deck> validator = new EntityChangeValidator<Deck>();
      validator.Register(nameof(Deck.Title), new DeckTitleValidator());
      CardTemplate template = new CardTemplate() { CardTemplateId = 1, Title = "test" };
      ApiConnectorMock mock = CreateMockForInitialize(false, true, null, new List<CardTemplate>() { template });
      DeckEditViewModel viewModel = new DeckEditViewModel(navigationManagerMock, mock, validator);
      await viewModel.InitializeAsync();

      viewModel.TitleProperty.Value = null;
      Assert.IsFalse(string.IsNullOrEmpty(viewModel.TitleProperty.ErrorText));
      viewModel.TitleProperty.Value = "test";
      Assert.IsTrue(string.IsNullOrEmpty(viewModel.TitleProperty.ErrorText));
    }

    /// <summary>
    /// Tests <see cref="DeckEditViewModel.CardTemplateTitleProperty"/> is validated
    /// </summary>
    [TestMethod]
    public async Task DoesValidateCardTemplateTitlePropertyTest()
    {
      ApiConnectorMock mock = CreateMockForInitialize(false, true, null, new List<CardTemplate>());
      DeckEditViewModel viewModel = new DeckEditViewModel(navigationManagerMock, mock, new EntityChangeValidator<Deck>());
      await viewModel.InitializeAsync();
      viewModel.CardTemplateTitleProperty.Value = null;
      Assert.IsFalse(string.IsNullOrEmpty(viewModel.CardTemplateTitleProperty.ErrorText));

      CardTemplate template = new CardTemplate() { CardTemplateId = 1, Title = "test" };
      mock = CreateMockForInitialize(false, true, null, new List<CardTemplate>() { template });
      viewModel = new DeckEditViewModel(navigationManagerMock, mock, new EntityChangeValidator<Deck>());
      await viewModel.InitializeAsync();
      viewModel.CardTemplateTitleProperty.Value = "test";
      Assert.IsTrue(string.IsNullOrEmpty(viewModel.CardTemplateTitleProperty.ErrorText));
    }

    private ApiConnectorMock CreateMockForInitialize(bool loadEntity, bool loadTemplatesSuccessful, Deck deck, List<CardTemplate> templates)
    {
      ApiConnectorMock mock = new ApiConnectorMock();
      if (loadEntity)
      {
        mock.Replies.Push(new ApiReply<Deck>()
        {
          WasSuccessful = true,
          Result = deck
        });
      }
      mock.Replies.Push(new ApiReply<List<CardTemplate>>()
      {
        ResultMessage = loadTemplatesSuccessful ? null : "test-error",
        WasSuccessful = loadTemplatesSuccessful,
        Result = templates,
      });
      return mock;
    }
  }
}