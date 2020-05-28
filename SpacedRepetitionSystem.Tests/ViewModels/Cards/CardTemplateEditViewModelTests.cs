using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpacedRepetitionSystem.Components.Edits;
using SpacedRepetitionSystem.Components.Middleware;
using SpacedRepetitionSystem.Components.Tests;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Entities.Validation.CardTemplates;
using SpacedRepetitionSystem.Entities.Validation.Core;
using SpacedRepetitionSystem.Utility.Notification;
using SpacedRepetitionSystem.Utility.Tests.Notification;
using SpacedRepetitionSystem.ViewModels.Cards;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Tests.ViewModels.Cards
{
  /// <summary>
  /// Testclas for <see cref="CardEditViewModel"/>
  /// </summary>
  [TestClass]
  public sealed class CardTemplateEditViewModelTests
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
    /// Tests the creation of a new deck on <see cref="CardEditViewModel.InitializeAsync"/>
    /// </summary>
    [TestMethod]
    public async Task CreateNewCardTemplateOnInitializeTest()
    {
      ApiConnectorMock mock = CreateMockForInitialize(false, null);
      CardTemplateEditViewModel viewModel
        = new CardTemplateEditViewModel(navigationManagerMock, mock, new EntityChangeValidator<CardTemplate>());
      bool result = await viewModel.InitializeAsync();

      Assert.IsTrue(result);
      Assert.IsNotNull(viewModel.Entity);
      Assert.AreEqual(2, viewModel.FieldDefinitions.Count);
      Assert.AreEqual(2, viewModel.Entity.FieldDefinitions.Count);
      Assert.AreEqual(2, viewModel.FieldNameProperties.Count);
      Assert.IsTrue(viewModel.Entity.FieldDefinitions[0].IsRequired);
      Assert.IsTrue(viewModel.Entity.FieldDefinitions[1].IsRequired);
    }

    /// <summary>
    /// Tests that all commands are initialized correctly
    /// </summary>
    [TestMethod]
    public async Task CommandsAreInitializedCorrectlyTest()
    {
      ApiConnectorMock mock = CreateMockForInitialize(false, null);
      CardTemplateEditViewModel viewModel
        = new CardTemplateEditViewModel(navigationManagerMock, mock, new EntityChangeValidator<CardTemplate>());
      await viewModel.InitializeAsync();

      Assert.IsNotNull(viewModel.AddFieldDefinitionCommand.ToolTip);
      Assert.IsNotNull(viewModel.RemoveFieldDefinitionCommand.ToolTip);
      Assert.IsNotNull(viewModel.CloseCommand.ToolTip);

      Assert.IsNotNull(viewModel.DeleteCommand.CommandText);
      Assert.IsNotNull(viewModel.DeleteCommand.ToolTip);

      Assert.IsNotNull(viewModel.SaveChangesCommand.CommandText);
      Assert.IsNotNull(viewModel.SaveChangesCommand.ToolTip);
    }

    /// <summary>
    /// Tests the behavior of <see cref="CardTemplateEditViewModel.RemoveFieldDefinitionCommand"/>
    /// </summary>
    [TestMethod]
    public async Task RemoveFieldCommandTest()
    {
      ApiConnectorMock mock = CreateMockForInitialize(false, null);
      CardTemplateEditViewModel viewModel
        = new CardTemplateEditViewModel(navigationManagerMock, mock, new EntityChangeValidator<CardTemplate>());
      await viewModel.InitializeAsync();

      //not enabled because requires 2 fields required
      Assert.IsFalse(viewModel.RemoveFieldDefinitionCommand.IsEnabled);
      viewModel.AddFieldDefinitionCommand.ExecuteCommand();
      Assert.IsTrue(viewModel.RemoveFieldDefinitionCommand.IsEnabled);

      viewModel.RemoveFieldDefinitionCommand.ExecuteCommand(0);
      Assert.AreEqual(2, viewModel.FieldDefinitions.Count);
      Assert.AreEqual(2, viewModel.FieldNameProperties.Count);
    }

    /// <summary>
    /// Tests the behavior of <see cref="CardTemplateEditViewModel.AddFieldDefinitionCommand"/>
    /// </summary>
    [TestMethod]
    public async Task AddFieldCommandTest()
    {
      ApiConnectorMock mock = CreateMockForInitialize(false, null);
      CardTemplateEditViewModel viewModel
        = new CardTemplateEditViewModel(navigationManagerMock, mock, new EntityChangeValidator<CardTemplate>());
      await viewModel.InitializeAsync();

      viewModel.AddFieldDefinitionCommand.ExecuteCommand();
      Assert.AreEqual(3, viewModel.FieldDefinitions.Count);
      Assert.AreEqual(3, viewModel.FieldNameProperties.Count);
      PropertyProxy proxy = viewModel.FieldNameProperties[2];
      Assert.AreEqual(nameof(CardFieldDefinition.FieldName), proxy.PropertyName);
      Assert.AreSame(viewModel.FieldDefinitions[2], proxy.Entity);
    }

    /// <summary>
    /// Tests <see cref="CardFieldDefinition.FieldName"/> is validated
    /// </summary>
    [TestMethod]
    public async Task DoesValidateFieldNamePropertiesTest()
    {
      ApiConnectorMock mock = CreateMockForInitialize(false, null);
      EntityChangeValidator<CardFieldDefinition> fieldValidator = new EntityChangeValidator<CardFieldDefinition>();
      fieldValidator.Register(nameof(CardFieldDefinition.FieldName), new CardFieldDefinitionFieldNameValidator());
      CardTemplateChangeValidator validator = new CardTemplateChangeValidator(fieldValidator);
      CardTemplateEditViewModel viewModel = new CardTemplateEditViewModel(navigationManagerMock, mock, validator);
      await viewModel.InitializeAsync();

      viewModel.FieldNameProperties[0].Value = null;
      Assert.IsFalse(string.IsNullOrEmpty(viewModel.FieldNameProperties[0].ErrorText));
      viewModel.FieldNameProperties[0].Value = "test";
      Assert.IsTrue(string.IsNullOrEmpty(viewModel.FieldNameProperties[0].ErrorText));
    }

    /// <summary>
    /// Tests <see cref="CardTemplateEditViewModel.TitleProperty"/> is validated
    /// </summary>
    [TestMethod]
    public async Task DoesValidateTitlePropertyTest()
    {
      ApiConnectorMock mock = CreateMockForInitialize(false, null);
      EntityChangeValidator<CardTemplate> validator = new EntityChangeValidator<CardTemplate>();
      validator.Register(nameof(CardTemplate.Title), new CardTemplateTitleValidator());
      CardTemplateEditViewModel viewModel = new CardTemplateEditViewModel(navigationManagerMock, mock, validator);
      await viewModel.InitializeAsync();
      viewModel.TitleProperty.Value = null;
      Assert.IsFalse(string.IsNullOrEmpty(viewModel.TitleProperty.ErrorText));

      mock = CreateMockForInitialize(false, null);
      viewModel = new CardTemplateEditViewModel(navigationManagerMock, mock, validator);
      await viewModel.InitializeAsync();
      viewModel.TitleProperty.Value = "test";
      Assert.IsTrue(string.IsNullOrEmpty(viewModel.TitleProperty.ErrorText));
    }

    private ApiConnectorMock CreateMockForInitialize(bool loadEntity, CardTemplate template)
    {
      ApiConnectorMock mock = new ApiConnectorMock();
      if (loadEntity)
      {
        mock.Replies.Push(new ApiReply<CardTemplate>()
        {
          WasSuccessful = true,
          Result = template
        });
      }
      return mock;
    }
  }
}