using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpacedRepetitionSystem.Components.Middleware;
using SpacedRepetitionSystem.Components.Tests;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Utility.Notification;
using SpacedRepetitionSystem.Utility.Tests.Notification;
using SpacedRepetitionSystem.ViewModels.Cards;
using System;
using SpacedRepetitionSystem.Utility.Extensions;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;

namespace SpacedRepetitionSystem.Tests.ViewModels.Cards
{
  /// <summary>
  /// Testclass for <see cref="PracticeDeckViewModel"/>
  /// </summary>
  [TestClass]
  public sealed class PracticeDeckViewModelTests
  {
    private readonly NavigationManagerMock navigationManagerMock = new NavigationManagerMock();
    private readonly NotificationProviderMock notificationProviderMock = new NotificationProviderMock();
    private CardTemplate template;
    private CardFieldDefinition fieldDefinition1;
    private CardFieldDefinition fieldDefinition2;
    private Deck deck;
    private Card card1;
    private Card card2;
    private CardField field1;
    private CardField field2;
    private CardField field3;
    private CardField field4;

    /// <summary>
    /// Initializes the tests
    /// </summary>
    [TestInitialize]
    public void TestInitialize()
    { 
      NotificationMessageProvider.Initialize(notificationProviderMock, 500000);

      template = new CardTemplate() { CardTemplateId = 1, Title = "template" };
      fieldDefinition1 = new CardFieldDefinition()
      {
        CardTemplate = template,
        CardTemplateId = 1,
        FieldId = 1,
        FieldName = "Front"
      };
      fieldDefinition2 = new CardFieldDefinition()
      {
        CardTemplate = template,
        CardTemplateId = 1,
        FieldId = 2,
        FieldName = "Back"
      };
      template.FieldDefinitions.Add(fieldDefinition1);
      template.FieldDefinitions.Add(fieldDefinition2);
      deck = new Deck() { DeckId = 1, DefaultCardTemplateId = 1, Title = "deck", DefaultCardTemplate = template };
      card1 = new Card() { CardId = 1, CardTemplateId = 1, CardTemplate = template, DeckId = 1, Deck = deck };
      field1 = new CardField() 
      { 
        CardFieldDefinition = fieldDefinition1,
        CardId = 1, 
        Card = card1, 
        CardTemplateId = 1, 
        CardTemplate = template, 
        FieldId = 1, 
        FieldName = "Front", 
        Value = "11",
        DueDate = DateTime.Today
      };
      field2 = new CardField() 
      {
        CardFieldDefinition = fieldDefinition2,
        CardId = 1, 
        Card = card1,
        CardTemplateId = 1, 
        CardTemplate = template, 
        FieldId = 2, 
        FieldName = "Back", 
        Value = "12",
        DueDate = DateTime.Today
      };
      card1.Fields.Add(field1);
      card1.Fields.Add(field2);
      card2 = new Card() { CardId = 2, CardTemplateId = 1, CardTemplate = template, DeckId = 1, Deck = deck };
      field3 = new CardField()
      {
        CardFieldDefinition = fieldDefinition1,
        CardId = 2,
        Card = card2,
        CardTemplateId = 1,
        CardTemplate = template,
        FieldId = 1,
        FieldName = "Front",
        Value = "21",
        DueDate = DateTime.Today
      };
      field4 = new CardField()
      {
        CardFieldDefinition = fieldDefinition2,
        CardId = 2,
        Card = card2,
        CardTemplateId = 1,
        CardTemplate = template,
        FieldId = 2,
        FieldName = "Back",
        Value = "22",
        DueDate = DateTime.Today.AddDays(1)
      };
      card2.Fields.Add(field3);
      card2.Fields.Add(field4);
      deck.Cards.Add(card1);
      deck.Cards.Add(card2);
    }

    /// <summary>
    /// Test that the data is loaded on <see cref="PracticeDeckViewModel"/>
    /// </summary>
    [TestMethod]
    public async Task LoadDataOnInitializeTest()
    {
      card1.Fields.Add(new CardField() { DueDate = DateTime.Today, FieldName = "FieldWithoutValue" });
      ApiConnectorMock mock = CreateMockForInitialize(true, deck);
      PracticeDeckViewModel viewModel = new PracticeDeckViewModel(navigationManagerMock, mock);
      bool result = await viewModel.InitializeAsync();

      Assert.IsTrue(result);
      Assert.AreSame(deck, viewModel.Entity);
      Assert.AreEqual(3, viewModel.PracticeFields.Count); //Only fields that are due and have a value
      Assert.IsTrue(viewModel.PracticeFields.Contains(field1));
      Assert.IsTrue(viewModel.PracticeFields.Contains(field2));
      Assert.IsTrue(viewModel.PracticeFields.Contains(field3));
      Assert.AreSame(viewModel.PracticeFields[0], viewModel.Current);
      Assert.AreEqual(viewModel.Current.FieldName, viewModel.CurrentFieldName);
      CardField expectedDisplayField = viewModel.Current.Card.Fields
        .Single(field => field.Value != null && field != viewModel.Current);
      Assert.AreSame(expectedDisplayField, viewModel.DisplayedCardField);
      Assert.IsFalse(viewModel.IsSummary);
      Assert.IsFalse(viewModel.IsShowingSolution);
    }

    /// <summary>
    /// Test that the correct title is used for the page
    /// </summary>
    [TestMethod]
    public async Task TitleTest()
    {
      ApiConnectorMock mock = CreateMockForInitialize(true, deck);
      PracticeDeckViewModel viewModel = new PracticeDeckViewModel(navigationManagerMock, mock);
      bool result = await viewModel.InitializeAsync();

      Assert.IsTrue(result);
      Assert.AreEqual(Messages.PracticePageTitle.FormatWith(deck.Title), viewModel.Title);
      viewModel.IsSummary = true;
      Assert.AreEqual(Messages.PracticePageSummaryTitle, viewModel.Title);
    }

    /// <summary>
    /// Test that all fields are loaded when no field is actually due
    /// </summary>
    [TestMethod]
    public async Task UseAllFieldsWhenNoFieldIsDueTest()
    {
      card1.Fields.Add(new CardField() { DueDate = DateTime.Today, FieldName = "FieldWithoutValue" });
      field1.DueDate = field2.DueDate = field3.DueDate = field4.DueDate = DateTime.Today.AddDays(-1);
      ApiConnectorMock mock = CreateMockForInitialize(true, deck);
      PracticeDeckViewModel viewModel = new PracticeDeckViewModel(navigationManagerMock, mock);
      bool result = await viewModel.InitializeAsync();

      Assert.IsTrue(result);
      Assert.AreSame(deck, viewModel.Entity);
      Assert.AreEqual(4, viewModel.PracticeFields.Count); //All fields that have a value
      Assert.IsTrue(viewModel.PracticeFields.Contains(field1));
      Assert.IsTrue(viewModel.PracticeFields.Contains(field3));
      Assert.AreSame(viewModel.PracticeFields[0], viewModel.Current);
    }

    /// <summary>
    /// Test that the initialize fails when an error is returned instead of the deck
    /// </summary>
    [TestMethod]
    public async Task InitializeFailsOnErrorWhenLoadingDeckTest()
    {
      ApiConnectorMock mock = CreateMockForInitialize(false, null);
      PracticeDeckViewModel viewModel = new PracticeDeckViewModel(navigationManagerMock, mock);
      bool result = await viewModel.InitializeAsync();

      Assert.IsFalse(result);
      Assert.AreEqual(NotificationKind.ErrorNotification, notificationProviderMock.NotificationKind);
      Assert.AreEqual("test-error", notificationProviderMock.Message);
    }

    /// <summary>
    /// Tests that all commands are initialized correctly
    /// </summary>
    [TestMethod]
    public async Task CommandsAreInitializedCorrectlyTest()
    {
      ApiConnectorMock mock = CreateMockForInitialize(true, deck);
      PracticeDeckViewModel viewModel = new PracticeDeckViewModel(navigationManagerMock, mock);
      await viewModel.InitializeAsync();

      Assert.IsNotNull(viewModel.CloseCommand.ToolTip);

      Assert.IsNotNull(viewModel.CloseSummaryCommand.CommandText);
      Assert.IsNotNull(viewModel.CloseSummaryCommand.ToolTip);
      Assert.AreEqual("/", viewModel.CloseSummaryCommand.TargetUri);

      Assert.IsNotNull(viewModel.EasyResultCommand.CommandText);
      Assert.IsNotNull(viewModel.EasyResultCommand.ToolTip);

      Assert.IsNotNull(viewModel.DifficultResultCommand.CommandText);
      Assert.IsNotNull(viewModel.DifficultResultCommand.ToolTip);

      Assert.IsNotNull(viewModel.DoesNotKnowResultCommand.CommandText);
      Assert.IsNotNull(viewModel.DoesNotKnowResultCommand.ToolTip);

      Assert.IsNotNull(viewModel.ShowSolutionCommand.CommandText);
      Assert.IsNotNull(viewModel.ShowSolutionCommand.ToolTip);

      Assert.IsNotNull(viewModel.NextCommand.CommandText);
      Assert.IsNotNull(viewModel.NextCommand.ToolTip);
    }

    /// <summary>
    /// Tests <see cref="PracticeDeckViewModel.ShowSolutionCommand"/> when input was correct
    /// </summary>
    [TestMethod]
    public async Task ShowSolutionCommandCorrectInputTest()
    { await ShowSolutionCommandTestCore(true, true); }

    /// <summary>
    /// Tests <see cref="PracticeDeckViewModel.ShowSolutionCommand"/> when not using input
    /// </summary>
    [TestMethod]
    public async Task ShowSolutionCommadWithoutInputTest()
    { await ShowSolutionCommandTestCore(false, false); }


    /// <summary>
    /// Tests <see cref="PracticeDeckViewModel.ShowSolutionCommand"/> when input was wrong
    /// </summary>
    [TestMethod]
    public async Task ShowSoulutionCommandWrongInputTest()
    { await ShowSolutionCommandTestCore(true, false); }

    /// <summary>
    /// Tests <see cref="PracticeDeckViewModel.EasyResultCommand"/>
    /// </summary>
    [TestMethod]
    public async Task EasyResultCommandCommandTest()
    { await ResultCommandTestCore(PracticeResultKind.Easy); }

    /// <summary>
    /// Tests <see cref="PracticeDeckViewModel.HardResultCommand"/>
    /// </summary>
    [TestMethod]
    public async Task HardResultCommandCommandTest()
    { await ResultCommandTestCore(PracticeResultKind.Hard); }

    /// <summary>
    /// Tests <see cref="PracticeDeckViewModel.DoesNotKnowResultCommand"/>
    /// </summary>
    [TestMethod]
    public async Task WrongResultCommandCommandTest()
    { await ResultCommandTestCore(PracticeResultKind.Wrong); }

    /// <summary>
    /// Tests <see cref="PracticeDeckViewModel.OnInputFinished"/> when input was correct
    /// </summary>
    [TestMethod]
    public async Task OnInputFinishedInputCorrectTest()
    { await OnInputFinishedTestCore(true); }

    /// <summary>
    /// Tests <see cref="PracticeDeckViewModel.OnInputFinished"/> when input was wrong
    /// </summary>
    [TestMethod]
    public async Task OnInputFinishedInputWrongTest()
    { await OnInputFinishedTestCore(false); }

    /// <summary>
    /// Tests <see cref="PracticeDeckViewModel.NextCommand"/>
    /// </summary>
    [TestMethod]
    public async Task NextCommandTest()
    {
      ApiConnectorMock mock = CreateMockForInitialize(true, deck);
      PracticeDeckViewModel viewModel = new PracticeDeckViewModel(navigationManagerMock, mock)
      {
        InputText = "test123",
        IsShowingSolution = true,
        WasInputCorrect = true
      };
      await viewModel.InitializeAsync();
      CardField previous = viewModel.Current;
      viewModel.NextCommand.ExecuteCommand();

      Assert.IsTrue(string.IsNullOrEmpty(viewModel.InputText));
      Assert.IsNull(viewModel.WasInputCorrect);
      Assert.IsFalse(viewModel.IsShowingSolution);
      Assert.AreNotSame(previous, viewModel.Current);
      Assert.IsFalse(viewModel.IsSummary);

      viewModel.NextCommand.ExecuteCommand();
      viewModel.NextCommand.ExecuteCommand();
      Assert.IsTrue(viewModel.IsSummary);
    }

    private async Task OnInputFinishedTestCore(bool expectedResult)
    {
      fieldDefinition1.ShowInputForPractice = fieldDefinition2.ShowInputForPractice = true;
      ApiConnectorMock mock = CreateMockForInitialize(true, deck);
      PracticeDeckViewModel viewModel = new PracticeDeckViewModel(navigationManagerMock, mock);
      await viewModel.InitializeAsync();
      mock.Replies.Push(new ApiReply() { WasSuccessful = true });
      long cardId = viewModel.Current.CardId;
      int fieldId = viewModel.Current.FieldId;
      viewModel.InputText = expectedResult ? viewModel.Current.Value : null;
      viewModel.OnInputFinished();

      Assert.AreEqual(expectedResult, viewModel.WasInputCorrect);
      Assert.IsTrue(viewModel.IsShowingSolution);
      TestReportedResult(viewModel, expectedResult ? PracticeResultKind.Easy : PracticeResultKind.Wrong, mock, cardId, fieldId);
    }

    private async Task ShowSolutionCommandTestCore(bool useInput, bool expectedResult)
    {
      if (useInput)
        fieldDefinition1.ShowInputForPractice = fieldDefinition2.ShowInputForPractice = true;
      ApiConnectorMock mock = CreateMockForInitialize(true, deck);
      PracticeDeckViewModel viewModel = new PracticeDeckViewModel(navigationManagerMock, mock);
      await viewModel.InitializeAsync();
      viewModel.InputText = expectedResult ? viewModel.Current.Value : null;
      viewModel.ShowSolutionCommand.ExecuteCommand();

      if (useInput)   
        Assert.AreEqual(expectedResult, viewModel.WasInputCorrect);
      Assert.IsTrue(viewModel.IsShowingSolution);
    }

    private async Task ResultCommandTestCore(PracticeResultKind expectedResult)
    {
      ApiConnectorMock mock = CreateMockForInitialize(true, deck);
      PracticeDeckViewModel viewModel = new PracticeDeckViewModel(navigationManagerMock, mock)
      { IsShowingSolution = true };
      await viewModel.InitializeAsync();
      mock.Replies.Push(new ApiReply() { WasSuccessful = true });
      long cardId = viewModel.Current.CardId;
      int fieldId = viewModel.Current.FieldId;
      CardField previousField = viewModel.Current;

      switch (expectedResult)
      {
        case PracticeResultKind.Easy:
          viewModel.EasyResultCommand.ExecuteCommand();
          break;
        case PracticeResultKind.Hard:
          viewModel.DifficultResultCommand.ExecuteCommand();
          break;
        case PracticeResultKind.Wrong:
          viewModel.DoesNotKnowResultCommand.ExecuteCommand();
          break;
        default:
          break;
      }

      TestReportedResult(viewModel, expectedResult, mock, cardId, fieldId);

      Assert.AreEqual(1, viewModel.PracticeResults.Count);
      Assert.AreEqual(1, viewModel.PracticeResults[cardId].FieldResults.Count);
      Assert.IsFalse(viewModel.IsShowingSolution);
      if (expectedResult == PracticeResultKind.Wrong)
        Assert.AreEqual(4, viewModel.PracticeFields.Count);
      else
      {
        Assert.AreEqual(3, viewModel.PracticeFields.Count);
        //Because field is added again when wrong or don't know could be same field again
        Assert.AreNotSame(previousField, viewModel.Current); 
      }
      Assert.IsNotNull(viewModel.Current);
      Assert.IsFalse(viewModel.IsSummary);
    }

    private void TestReportedResult(PracticeDeckViewModel viewModel, PracticeResultKind expectedResult,
      ApiConnectorMock mock, long cardId, int fieldId)
    {
      PracticeHistoryEntry entry = mock.Parameters.Pop() as PracticeHistoryEntry;
      Assert.AreEqual(HttpMethod.Post, mock.Methods.Pop());
      Assert.AreEqual(DateTime.Today, entry.PracticeDate);
      Assert.AreEqual(cardId, entry.CardId);
      Assert.AreEqual(fieldId, entry.FieldId);
      Assert.AreEqual(deck.DeckId, entry.DeckId);

      int correct = expectedResult == PracticeResultKind.Easy ? 1 : 0;
      int hard = expectedResult == PracticeResultKind.Hard ? 1 : 0;
      int wrong = expectedResult == PracticeResultKind.Wrong ? 1 : 0;
      Assert.AreEqual(correct, entry.CorrectCount);
      Assert.AreEqual(hard, entry.HardCount);
      Assert.AreEqual(wrong, entry.WrongCount);
      Assert.AreEqual(correct, viewModel.PracticeResults[cardId].Correct);
      Assert.AreEqual(hard, viewModel.PracticeResults[cardId].Difficult);
      Assert.AreEqual(wrong, viewModel.PracticeResults[cardId].Wrong);
      Assert.AreEqual(correct, viewModel.PracticeResults[cardId].FieldResults[fieldId].Correct);
      Assert.AreEqual(hard, viewModel.PracticeResults[cardId].FieldResults[fieldId].Difficult);
      Assert.AreEqual(wrong, viewModel.PracticeResults[cardId].FieldResults[fieldId].Wrong);
    }

    private ApiConnectorMock CreateMockForInitialize(bool loadEntity, Deck deck)
    {
      ApiConnectorMock mock = new ApiConnectorMock();
      mock.Replies.Push(new ApiReply<Deck>()
      {
        ResultMessage = loadEntity ? null : "test-error",
        WasSuccessful = loadEntity,
        Result = deck,
      });
      return mock;
    }
  }
}