using Microsoft.AspNetCore.Components;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpacedRepetitionSystem.Components.Middleware;
using SpacedRepetitionSystem.Components.ViewModels;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Entities.Validation.Core;
using SpacedRepetitionSystem.Utility.Notification;
using SpacedRepetitionSystem.Utility.Tests.Notification;
using SpacedRepetitionSystem.Utility.Extensions;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using SpacedRepetitionSystem.Components.Edits;

namespace SpacedRepetitionSystem.Components.Tests.ViewModels
{
  /// <summary>
  /// Testclass for <see cref="EditViewModelBase{TEntity}"/>
  /// </summary>
  [TestClass]
  public sealed class EditViewModelBaseTests
  {
    private static NavigationManagerMock navigationManagerMock;
    private static ApiConnectorMock apiConnectorMock;
    private static NotificationProviderMock notificationProviderMock;
    private static readonly Card card = new Card();
    private static readonly Card newCard = new Card();

    private class TestViewModel : EditViewModelBase<Card>
    {
      public bool CreateNewEntityCalled { get; set; }

      public TestViewModel(NavigationManager navigationManager, IApiConnector apiConnector, EntityChangeValidator<Card> changeValidator)
        : base(navigationManager, apiConnector, changeValidator)
      { }

      protected override void CreateNewEntity()
      {
        CreateNewEntityCalled = true;
        Entity = newCard;
      }

      public void CallRegisterPropertyProxy(PropertyProxy proxy) 
        => RegisterPropertyProxy(proxy);

      public bool GetIsNewEntity() => IsNewEntity;
    }

    private class TestChangeValidator : EntityChangeValidator<Card>
    {
      public string PropertyName { get; set; }

      public object Entity { get; set; }

      public object NewValue { get; set; }

      public override string Validate<TProperty>(string propertyName, object entity, TProperty newValue)
      {
        PropertyName = propertyName;
        Entity = entity;
        NewValue = newValue;
        return "test";
      }
    }

    /// <summary>
    /// Method for initializing the test
    /// </summary>
    [TestInitialize]
    public void TestInitialize()
    {
      notificationProviderMock = new NotificationProviderMock();
      NotificationMessageProvider.Initialize(notificationProviderMock, 1000000);
      apiConnectorMock = new ApiConnectorMock();
      navigationManagerMock = new NavigationManagerMock();
    }

    /// <summary>
    /// Tests <see cref="EditViewModelBase{TEntity}.RegisterPropertyProxy(PropertyProxy)"/>
    /// </summary>
    [TestMethod]
    public async Task SavingEntitySetsIsNewEntityToFalseTest()
    {
      TestViewModel viewModel = new TestViewModel(navigationManagerMock, apiConnectorMock, new EntityChangeValidator<Card>());
      apiConnectorMock.Replies.Push(new ApiReply() { WasSuccessful = true });
      await viewModel.InitializeAsync();
      viewModel.SaveChangesCommand.ExecuteCommand();

      Assert.IsFalse(viewModel.GetIsNewEntity());
    }

    /// <summary>
    /// Tests <see cref="EditViewModelBase{TEntity}.RegisterPropertyProxy(PropertyProxy)"/>
    /// </summary>
    [TestMethod]
    public void RegisterPropertyProxyTest()
    {
      PropertyProxy proxy = new PropertyProxy(() => card.CardId.ToString(),
        (value) => card.CardId = long.Parse(value), nameof(Card.CardId), card);
      TestChangeValidator changeValidator = new TestChangeValidator();
      TestViewModel viewModel = new TestViewModel(navigationManagerMock, apiConnectorMock, changeValidator);
      viewModel.CallRegisterPropertyProxy(proxy);
      string result = proxy.Validator.Invoke("12", card);

      Assert.AreEqual("test", result);
      Assert.AreEqual(nameof(CardField.CardId), changeValidator.PropertyName);
      Assert.AreEqual("12", changeValidator.NewValue);
      Assert.AreSame(card, changeValidator.Entity);
    }

    /// <summary>
    /// Tests that a new entity is created on <see cref="EditViewModelBase{TEntity}.InitializeAsync"/> when the id is null
    /// </summary>
    [TestMethod]
    public async Task DoesCreateNewEntityOnInitializeTest()
    {
      TestViewModel viewModel = new TestViewModel(navigationManagerMock, apiConnectorMock, new EntityChangeValidator<Card>());
      bool result = await viewModel.InitializeAsync();

      Assert.IsTrue(result);
      Assert.AreSame(newCard, viewModel.Entity);
      Assert.IsTrue(viewModel.CreateNewEntityCalled);
      Assert.IsTrue(viewModel.GetIsNewEntity());
    }

    /// <summary>
    /// Tests that a new entity is loaded on <see cref="EditViewModelBase{TEntity}.InitializeAsync"/> when id is set
    /// </summary>
    [TestMethod]
    public async Task LoadEntityOnInitializeSuccessfullyTest()
    {
      apiConnectorMock.Replies.Push(new ApiReply<Card>() { WasSuccessful = true, Result = card });
      await LoadOnInitializeTestCore(true, null);
    }

    /// <summary>
    /// Tests <see cref="EditViewModelBase{TEntity}.InitializeAsync"/> when a generic error is returned
    /// </summary>
    [TestMethod]
    public async Task LoadEntityOnInitializeGenericErrorTest()
    {
      apiConnectorMock.Replies.Push(new ApiReply<Card>() { WasSuccessful = false, ResultMessage = "test" });
      await LoadOnInitializeTestCore(false, "test");
    }

    /// <summary>
    /// Tests <see cref="EditViewModelBase{TEntity}.InitializeAsync"/> when a not found error is returned
    /// </summary>
    [TestMethod]
    public async Task LoadEntityOnInitializeNotFoundErrorTest()
    {
      apiConnectorMock.Replies.Push(new ApiReply<Card>() { WasSuccessful = false, StatusCode = HttpStatusCode.NotFound });
      await LoadOnInitializeTestCore(false, Errors.EntityDoesNotExist.FormatWith("Card", 12));
    }

    private async Task LoadOnInitializeTestCore(bool successful, string expectedError)
    {
      TestViewModel viewModel = new TestViewModel(navigationManagerMock, apiConnectorMock, new EntityChangeValidator<Card>())
      { Id = 12 };
      bool result = await viewModel.InitializeAsync();
      Assert.AreEqual(successful, result);
      Assert.AreEqual(12, apiConnectorMock.Parameter);
      Assert.AreEqual(apiConnectorMock.Method, HttpMethod.Get);
      Assert.IsFalse(viewModel.GetIsNewEntity());
      if (successful)
        Assert.AreSame(card, viewModel.Entity);
      else
      {
        Assert.IsNull(viewModel.Entity);
        Assert.AreEqual(NotificationKind.ErrorNotification, notificationProviderMock.NotificationKind);
        Assert.AreEqual(expectedError, notificationProviderMock.Message);
      }
    }
  }
}