using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpacedRepetitionSystem.Components.Middleware;
using SpacedRepetitionSystem.Components.Tests;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Entities.Validation.Cards;
using SpacedRepetitionSystem.Entities.Validation.Core;
using SpacedRepetitionSystem.Utility.Notification;
using SpacedRepetitionSystem.Utility.Tests.Notification;
using SpacedRepetitionSystem.ViewModels.Cards;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Tests.ViewModels.Cards
{
  /// <summary>
  /// Testclass for <see cref="CardEditViewModel"/>
  /// </summary>
  [TestClass]
  public sealed class CardEditViewModelTests
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
      Deck deck = new Deck() { DeckId = 1, Title = "test", DefaultCardTemplateId = 1 };
      CardFieldDefinition field = new CardFieldDefinition()
      { CardTemplateId = 1, FieldId = 1, FieldName = "Field 1" };
      CardTemplate template = new CardTemplate() { CardTemplateId = 1, Title = "test1" };
      template.FieldDefinitions.Add(field);

      ApiConnectorMock mock = CreateMockForInitialize(false, true, deck, null, new List<CardTemplate>() { template });
      CardEditViewModel viewModel = new CardEditViewModel(navigationManagerMock, mock, new EntityChangeValidator<Card>());
      bool result = await viewModel.InitializeAsync();

      Assert.IsTrue(result);
      Assert.AreEqual(1, viewModel.AvailableCardTemplates.Count);
      Assert.AreEqual("test1", viewModel.AvailableCardTemplates[0]);
      Assert.AreEqual(1, viewModel.CardTemplateId);
      Assert.AreEqual("test1", viewModel.CardTemplateTitle);
      Assert.AreEqual(1, viewModel.Entity.Fields.Count);
      Assert.AreEqual(1, viewModel.FieldValueProperties.Count);
    }

    /// <summary>
    /// Tests the behavior when an error is returned while loading the available templatzes when the view model is initialized
    /// </summary>
    [TestMethod]
    public async Task LoadsAvailableCardTemplatesOnInitializeErrorTest()
    {
      Deck deck = new Deck() { DeckId = 1, Title = "test", DefaultCardTemplateId = 2 };
      CardTemplate template1 = new CardTemplate() { CardTemplateId = 1, Title = "test" };
      ApiConnectorMock mock = CreateMockForInitialize(false, false, deck, null, new List<CardTemplate>() { template1 });
      CardEditViewModel viewModel = new CardEditViewModel(navigationManagerMock, mock, new EntityChangeValidator<Card>());
      bool result = await viewModel.InitializeAsync();

      Assert.IsFalse(result);
      Assert.AreEqual(0, viewModel.AvailableCardTemplates.Count);
      Assert.AreEqual(NotificationKind.ErrorNotification, notificationProviderMock.NotificationKind);
      Assert.AreEqual("test-error", notificationProviderMock.Message);
    }

    /// <summary>
    /// Tests that <see cref="CardEditViewModel.InitializeAsync"/> fails when the deckId is not the same as the cards deck id
    /// </summary>
    [TestMethod]
    public async Task InitializeFailsForWrongDeckIdTest()
    {
      Deck deck = new Deck() { DeckId = 1, Title = "test", DefaultCardTemplateId = 2 };
      CardTemplate template1 = new CardTemplate() { CardTemplateId = 1, Title = "test" };
      ApiConnectorMock mock = CreateMockForInitialize(true, true, deck, new Card() { DeckId = 1 }, new List<CardTemplate>() { template1 });
      CardEditViewModel viewModel = new CardEditViewModel(navigationManagerMock, mock, new EntityChangeValidator<Card>())
      { Id = 1, DeckId = 2 };
      bool result = await viewModel.InitializeAsync();

      Assert.IsFalse(result);
      Assert.AreEqual(NotificationKind.ErrorNotification, notificationProviderMock.NotificationKind);
      Assert.IsFalse(string.IsNullOrEmpty(notificationProviderMock.Message));
    }

    /// <summary>
    /// Tests the creation of a new deck on <see cref="CardEditViewModel.InitializeAsync"/>
    /// </summary>
    [TestMethod]
    public async Task CreateNewCardOnInitializeTest()
    {
      Deck deck = new Deck() { DeckId = 1, Title = "test", DefaultCardTemplateId = 2 };
      CardTemplate template1 = new CardTemplate() { CardTemplateId = 1, Title = "test1" };
      CardTemplate template2 = new CardTemplate() { CardTemplateId = 2, Title = "test2" };
      ApiConnectorMock mock = CreateMockForInitialize(false, true, deck, null, new List<CardTemplate>() { template1, template2 });
      CardEditViewModel viewModel = new CardEditViewModel(navigationManagerMock, mock, new EntityChangeValidator<Card>());
      bool result = await viewModel.InitializeAsync();

      Assert.IsTrue(result);
      Assert.IsNotNull(viewModel.Entity);
      //Load default template
      Assert.AreEqual(2, viewModel.CardTemplateId);
      Assert.AreEqual(2, viewModel.Entity.CardTemplateId);
      Assert.AreEqual("test2", viewModel.CardTemplateTitle);
    }

    /// <summary>
    /// Tests that the tags are set on <see cref="CardEditViewModel.InitializeAsync"/>
    /// </summary>
    [TestMethod]
    public async Task SetsTagsOnInitializeTest()
    {
      Deck deck = new Deck() { DeckId = 1, Title = "test", DefaultCardTemplateId = 1 };
      CardTemplate template1 = new CardTemplate() { CardTemplateId = 1, Title = "test1" };
      Card card = new Card() { CardId = 1, CardTemplateId = 1, DeckId = 1, Tags = "tag1, tag2" };
      ApiConnectorMock mock = CreateMockForInitialize(true, true, deck, card, new List<CardTemplate>() { template1 });
      CardEditViewModel viewModel = new CardEditViewModel(navigationManagerMock, mock, new EntityChangeValidator<Card>())
      { Id = 1, DeckId = 1 };
      bool result = await viewModel.InitializeAsync();

      Assert.IsTrue(result);
      Assert.AreEqual(2, viewModel.Tags.Count);
      Assert.AreEqual("tag1", viewModel.Tags[0]);
      Assert.AreEqual("tag2", viewModel.Tags[1]);
    }

    /// <summary>
    /// Tests that the tags are updated
    /// </summary>
    [TestMethod]
    public async Task UpdateTagsTest()
    {
      Deck deck = new Deck() { DeckId = 1, Title = "test", DefaultCardTemplateId = 1 };
      CardTemplate template1 = new CardTemplate() { CardTemplateId = 1, Title = "test1" };
      Card card = new Card() { CardId = 1, CardTemplateId = 1, DeckId = 1 };
      ApiConnectorMock mock = CreateMockForInitialize(true, true, deck, card, new List<CardTemplate>() { template1 });
      CardEditViewModel viewModel = new CardEditViewModel(navigationManagerMock, mock, new EntityChangeValidator<Card>())
      { Id = 1, DeckId = 1 };
      bool result = await viewModel.InitializeAsync();

      Assert.IsTrue(result);
      Assert.AreEqual(0, viewModel.Tags.Count);
      viewModel.Tags.Add("tag1");
      Assert.AreEqual("tag1", viewModel.Entity.Tags);
      viewModel.Tags.Add("tag2");
      Assert.AreEqual("tag1,tag2", viewModel.Entity.Tags);
      viewModel.Tags.RemoveAt(0);
      Assert.AreEqual("tag2", viewModel.Entity.Tags);
    }

    /// <summary>
    /// Tests that fields are updated when the card template is changed
    /// </summary>
    [TestMethod]
    public async Task ChangeCardTemplateUpdatesFieldsTest()
    {
      Deck deck = new Deck() { DeckId = 1, Title = "test", DefaultCardTemplateId = 1 };
      CardTemplate template1 = new CardTemplate() { CardTemplateId = 1, Title = "test1" };
      CardTemplate template2 = new CardTemplate() { CardTemplateId = 2, Title = "test2" };
      template2.FieldDefinitions.Add(new CardFieldDefinition() { FieldId = 1, FieldName = "test" });
      Card card = new Card() { CardId = 1, CardTemplateId = 1, DeckId = 1 };
      ApiConnectorMock mock = CreateMockForInitialize(true, true, deck, card, new List<CardTemplate>() { template1, template2 });
      CardEditViewModel viewModel = new CardEditViewModel(navigationManagerMock, mock, new EntityChangeValidator<Card>())
      { Id = 1, DeckId = 1 };
      bool result = await viewModel.InitializeAsync();

      Assert.IsTrue(result);
      viewModel.CardTemplateId = template2.CardTemplateId;
      Assert.AreEqual(1, viewModel.Entity.Fields.Count);
      Assert.AreEqual(1, viewModel.FieldValueProperties.Count);
    }

    /// <summary>
    /// Tests that all commands are initialized correctly
    /// </summary>
    [TestMethod]
    public async Task CommandsAreInitializedCorrectlyTest()
    {
      Deck deck = new Deck() { DeckId = 1, Title = "test", DefaultCardTemplateId = 2 };
      CardTemplate template1 = new CardTemplate() { CardTemplateId = 1, Title = "test" };
      ApiConnectorMock mock = CreateMockForInitialize(false, true, deck, null, new List<CardTemplate>() { template1 });
      CardEditViewModel viewModel = new CardEditViewModel(navigationManagerMock, mock, new EntityChangeValidator<Card>());
      await viewModel.InitializeAsync();

      Assert.IsNotNull(viewModel.ShowStatisticsCommand.CommandText);
      Assert.IsNotNull(viewModel.ShowStatisticsCommand.ToolTip);
      Assert.AreEqual("/Statistics", viewModel.ShowStatisticsCommand.TargetUri);
      Assert.IsTrue(viewModel.ShowStatisticsCommand.IsRelative);

      Assert.IsNotNull(viewModel.DeleteCommand.CommandText);
      Assert.IsNotNull(viewModel.DeleteCommand.ToolTip);

      Assert.IsNotNull(viewModel.SaveChangesCommand.CommandText);
      Assert.IsNotNull(viewModel.SaveChangesCommand.ToolTip);
    }

    /// <summary>
    /// Tests the enabled bahavior of the commands
    /// </summary>
    [TestMethod]
    public async Task CommandsEnabledTest()
    {
      CardTemplate template = new CardTemplate() { CardTemplateId = 1, Title = "test" };
      Deck deck = new Deck() { DeckId = 1, Title = "test", DefaultCardTemplateId = 1 };

      //New Entity
      ApiConnectorMock mock = CreateMockForInitialize(false, true, deck, null, new List<CardTemplate>() { template });
      CardEditViewModel viewModel = new CardEditViewModel(navigationManagerMock, mock, new EntityChangeValidator<Card>());
      await viewModel.InitializeAsync();
      Assert.IsFalse(viewModel.ShowStatisticsCommand.IsEnabled);
      Assert.IsFalse(viewModel.DeleteCommand.IsEnabled);
      Assert.IsTrue(viewModel.SaveChangesCommand.IsEnabled);
      Assert.IsTrue(viewModel.CloseCommand.IsEnabled);

      //Existing entity
      Card card = new Card() { DeckId = 1, CardId = 1, Deck = deck };
      mock = CreateMockForInitialize(true, true, deck, card, new List<CardTemplate>() { template });
      viewModel = new CardEditViewModel(navigationManagerMock, mock, new EntityChangeValidator<Card>())
      { Id = 1, DeckId = 1 };
      await viewModel.InitializeAsync();
      Assert.IsTrue(viewModel.ShowStatisticsCommand.IsEnabled);
      Assert.IsTrue(viewModel.DeleteCommand.IsEnabled);
      Assert.IsTrue(viewModel.SaveChangesCommand.IsEnabled);
      Assert.IsTrue(viewModel.CloseCommand.IsEnabled);
    }

    /// <summary>
    /// Tests <see cref="CardEditViewModel.TitleProperty"/> is validated
    /// </summary>
    [TestMethod]
    public async Task DoesValidateFieldValuePropertiesTest()
    {
      Deck deck = new Deck() { DeckId = 1, Title = "test", DefaultCardTemplateId = 1 };
      CardFieldDefinition field = new CardFieldDefinition()
      { CardTemplateId = 1, FieldId = 1, FieldName = "Field 1", IsRequired = true };
      CardTemplate template = new CardTemplate() { CardTemplateId = 1, Title = "test1" };
      template.FieldDefinitions.Add(field);
      EntityChangeValidator<CardField> fieldChangeValidator = new EntityChangeValidator<CardField>();
      fieldChangeValidator.Register(nameof(CardField.Value), new CardFieldValueValidator());
      CardChangeValidator validator = new CardChangeValidator(fieldChangeValidator);
      ApiConnectorMock mock = CreateMockForInitialize(false, true, deck, null, new List<CardTemplate>() { template });
      CardEditViewModel viewModel = new CardEditViewModel(navigationManagerMock, mock, validator);
      await viewModel.InitializeAsync();

      viewModel.FieldValueProperties[0].Value = null;
      Assert.IsFalse(string.IsNullOrEmpty(viewModel.FieldValueProperties[0].ErrorText));
      viewModel.FieldValueProperties[0].Value = "test";
      Assert.IsTrue(string.IsNullOrEmpty(viewModel.FieldValueProperties[0].ErrorText));
    }

    /// <summary>
    /// Tests <see cref="CardEditViewModel.CardTemplateTitleProperty"/> is validated
    /// </summary>
    [TestMethod]
    public async Task DoesValidateCardTemplateTitlePropertyTest()
    {
      Deck deck = new Deck() { DeckId = 1, Title = "test", DefaultCardTemplateId = 2 };
      CardTemplate template1 = new CardTemplate() { CardTemplateId = 1, Title = "test" };
      ApiConnectorMock mock = CreateMockForInitialize(false, true, deck, null, new List<CardTemplate>() { template1 });
      CardEditViewModel viewModel = new CardEditViewModel(navigationManagerMock, mock, new EntityChangeValidator<Card>());
      await viewModel.InitializeAsync();
      viewModel.CardTemplateTitleProperty.Value = null;
      Assert.IsFalse(string.IsNullOrEmpty(viewModel.CardTemplateTitleProperty.ErrorText));

      mock = CreateMockForInitialize(false, true, deck, null, new List<CardTemplate>() { template1 });
      viewModel = new CardEditViewModel(navigationManagerMock, mock, new EntityChangeValidator<Card>());
      await viewModel.InitializeAsync();
      viewModel.CardTemplateTitleProperty.Value = "test";
      Assert.IsTrue(string.IsNullOrEmpty(viewModel.CardTemplateTitleProperty.ErrorText));
    }

    private ApiConnectorMock CreateMockForInitialize(bool loadEntity, bool loadTemplatesSuccessful, Deck deck, Card card, List<CardTemplate> templates)
    {
      ApiConnectorMock mock = new ApiConnectorMock();
      mock.Replies.Push(new ApiReply<Deck>()
      {
        WasSuccessful = true,
        Result = deck
      });
      if (loadEntity)
      {
        mock.Replies.Push(new ApiReply<Card>()
        {
          WasSuccessful = true,
          Result = card
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