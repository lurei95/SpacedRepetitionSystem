﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Entities.Entities.Security;
using SpacedRepetitionSystem.WebAPI.Controllers.Cards;
using SpacedRepetitionSystem.WebAPI.Validation.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.WebApi.Tests.Controllers.Cards
{
  /// <summary>
  /// Testclass for <see cref="PracticeHistoryEntriesController"/>
  /// </summary>
  [TestClass]
  public sealed class PracticeHistoryEntriesControllerTests : ControllerTestBase
  {
    private static User otherUser;
    private static CardField field1;
    private static CardField field2;
    private static CardField field3;
    private static CardField field4;
    private static PracticeHistoryEntry entry1;
    private static PracticeHistoryEntry entry2;
    private static PracticeHistoryEntry entry3;
    private static PracticeHistoryEntry otherUserEntry;

    /// <summary>
    /// Creates the test data when the class is initialized
    /// </summary>
    /// <param name="context">TestContext</param>
    [ClassInitialize]
#pragma warning disable IDE0060 // Remove unused parameter
    public static void ClassInitialize(TestContext context)
#pragma warning restore IDE0060 // Remove unused parameter
    {
      otherUser = new User()
      {
        UserId = Guid.NewGuid(),
        Email = "test1@test1.com",
        Password = "test1"
      };
      entry1 = new PracticeHistoryEntry()
      {
        CardId = 1,
        DeckId = 1,
        FieldId = 1,
        PracticeDate = DateTime.Today,
        PracticeHistoryEntryId = 1,
        UserId = User.UserId,
        CorrectCount = 1,
        HardCount = 2,
        WrongCount = 3
      };
      entry2 = new PracticeHistoryEntry()
      {
        CardId = 2,
        DeckId = 2,
        FieldId = 1,
        PracticeDate = DateTime.Today,
        PracticeHistoryEntryId = 2,
        UserId = User.UserId
      };
      entry3 = new PracticeHistoryEntry()
      {
        CardId = 1,
        DeckId = 1,
        FieldId = 3,
        PracticeDate = DateTime.Today,
        PracticeHistoryEntryId = 3,
        UserId = User.UserId,
        CorrectCount = 2,
        HardCount = 4,
        WrongCount = 6
      };
      otherUserEntry = new PracticeHistoryEntry()
      {
        CardId = 1,
        DeckId = 1,
        FieldId = 2,
        PracticeDate = DateTime.Today,
        PracticeHistoryEntryId = 4,
        UserId = otherUser.UserId
      };
      field1 = new CardField()
      {
        FieldName = "Field 1",
        CardId = 1,
        FieldId = 1,
        ProficiencyLevel = 3
      };
      field2 = new CardField()
      {
        FieldName = "Field 12",
        CardId = 1,
        FieldId = 2,
        ProficiencyLevel = 3
      };
      field3 = new CardField()
      {
        FieldName = "Field 2",
        CardId = 1,
        FieldId = 3,
        ProficiencyLevel = 3
      };
      field4 = new CardField()
      {
        FieldName = "Field 2",
        CardId = 2,
        FieldId = 1,
        ProficiencyLevel = 3
      };
    }

    ///<inheritdoc/>
    [TestInitialize]
    public override void TestInitialize()
    {
      base.TestInitialize();
      CreateData(context =>
      {
        context.Add(otherUser);
        context.Add(field1);
        context.Add(field2);
        context.Add(field3);
        context.Add(field4);
        context.Add(entry1);
        context.Add(entry2);
        context.Add(entry3);
        context.Add(otherUserEntry);
      });
    }

    /// <summary>
    /// Tests <see cref="PracticeHistoryEntriesController.GetAsync(long)"/>
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task GetEntryByIdTest()
    {
      using DbContext context = CreateContext();
      PracticeHistoryEntriesController controller = CreateController(context);

      //get entry successfully
      ActionResult<PracticeHistoryEntry> result = await controller.GetAsync(1);
      Assert.IsNotNull(result.Value);
      Assert.AreEqual(field1.FieldId, result.Value.Field.FieldId);

      //Entry of other user -> unauthorized
      result = await controller.GetAsync(4);
      Assert.IsTrue(result.Result is UnauthorizedResult);

      //Entry does not exist -> not found
      result = await controller.GetAsync(5);
      Assert.IsTrue(result.Result is NotFoundResult);
    }

    /// <summary>
    /// Tests <see cref="PracticeHistoryEntriesController.GetAsync(IDictionary{string, object})/>
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task GetEntriesTest()
    {
      using DbContext context = CreateContext();
      PracticeHistoryEntriesController controller = CreateController(context);
      Dictionary<string, object> parameters = new Dictionary<string, object>();
      
      //Get all entries of user
      ActionResult<List<PracticeHistoryEntry>> result = await controller.GetAsync(parameters);
      Assert.AreEqual(3, result.Value.Count);

      //Get all entries of a card
      parameters.Add(nameof(Card.CardId), (long)1);
      result = await controller.GetAsync(parameters);
      Assert.AreEqual(2, result.Value.Count);

      //Get all entries of a deck
      parameters.Clear();
      parameters.Add(nameof(Deck.DeckId), (long)2);
      result = await controller.GetAsync(parameters);
      Assert.AreEqual(1, result.Value.Count);

      //Get all entries of a field
      parameters.Clear();
      parameters.Add(nameof(CardField.FieldId), 3);
      result = await controller.GetAsync(parameters);
      Assert.AreEqual(1, result.Value.Count);

      //Get Problem words
      parameters.Clear();
      parameters.Add(nameof(PracticeHistoryEntriesController.ProblemWords), null);
      parameters.Add(nameof(Card.CardId), (long)1);
      result = await controller.GetAsync(parameters);
      Assert.AreEqual(1, result.Value.Count);
      Assert.AreEqual(3, result.Value[0].CorrectCount);
      Assert.AreEqual(6, result.Value[0].HardCount);
      Assert.AreEqual(9, result.Value[0].WrongCount);
    }

    /// <summary>
    /// Tests <see cref="PracticeHistoryEntriesController.PostAsync(PracticeHistoryEntry entity)/>
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task PostEntryTest()
    {
      using DbContext context = CreateContext();
      PracticeHistoryEntriesController controller = CreateController(context);

      //null as parameter -> bad request
      ActionResult<PracticeHistoryEntry> result = await controller.PostAsync(null);
      Assert.IsTrue(result.Result is BadRequestResult);

      //Same day -> update existing
      PracticeHistoryEntry newEntry = new PracticeHistoryEntry()
      {
        CardId = 1,
        DeckId = 1,
        FieldId = 1,
        PracticeDate = DateTime.Today,
        PracticeHistoryEntryId = 5,
        UserId = User.UserId,
        CorrectCount = 0,
        HardCount = 0,
        WrongCount = 1
      };
      result = await controller.PostAsync(newEntry);
      Assert.IsNotNull(result.Value);
      newEntry = context.Find<PracticeHistoryEntry>((long)5);
      Assert.IsNull(newEntry);
      newEntry = context.Find<PracticeHistoryEntry>((long)1);
      Assert.AreEqual(4, newEntry.WrongCount);
      CardField field1 = context.Find<CardField>((long)1, 1);
      Assert.AreEqual(1, field1.ProficiencyLevel);
      Assert.AreEqual(DateTime.Today.AddDays(1).Date, field1.DueDate.Date);

      //other day -> new entry
      newEntry = new PracticeHistoryEntry()
      {
        CardId = 1,
        DeckId = 1,
        FieldId = 1,
        PracticeDate = DateTime.Today.AddDays(1),
        PracticeHistoryEntryId = 5,
        UserId = User.UserId,
        CorrectCount = 1,
        HardCount = 0,
        WrongCount = 0
      };
      result = await controller.PostAsync(newEntry);
      Assert.IsNotNull(result.Value);
      newEntry = context.Find<PracticeHistoryEntry>((long)5);
      Assert.AreEqual(1, newEntry.CorrectCount);
      field1 = context.Find<CardField>((long)1, 1);
      Assert.AreEqual(2, field1.ProficiencyLevel);
      Assert.AreEqual(DateTime.Today.AddDays(3).Date, field1.DueDate.Date);

      //other day -> new entry
      newEntry = new PracticeHistoryEntry()
      {
        CardId = 1,
        DeckId = 1,
        FieldId = 1,
        PracticeDate = DateTime.Today.AddDays(1),
        PracticeHistoryEntryId = 6,
        UserId = User.UserId,
        CorrectCount = 0,
        HardCount = 1,
        WrongCount = 0
      };
      result = await controller.PostAsync(newEntry);
      Assert.IsNotNull(result.Value);
      field1 = context.Find<CardField>((long)1, 1);
      Assert.AreEqual((long)1, field1.ProficiencyLevel);
      Assert.AreEqual(DateTime.Today.AddDays(2).Date, field1.DueDate.Date);
    }

    /// <summary>
    /// Tests <see cref="PracticeHistoryEntriesController.DeleteAsync(PracticeHistoryEntry entity)/>
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task DeleteEntryNotSupportedTest()
    {
      using DbContext context = CreateContext();
      PracticeHistoryEntriesController controller = CreateController(context);

      bool wasThrown = false;
      try
      {
        IActionResult result = await controller.DeleteAsync(entry1);
      }
      catch (NotSupportedException) { wasThrown = true; }
      Assert.IsTrue(wasThrown);
    }

    /// <summary>
    /// Tests <see cref="PracticeHistoryEntriesController.PutAsync(PracticeHistoryEntry entity)/>
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task PutEntryNotSupportedTest()
    {
      using DbContext context = CreateContext();
      PracticeHistoryEntriesController controller = CreateController(context);

      bool wasThrown = false;
      try
      {
        ActionResult<PracticeHistoryEntry> result = await controller.PutAsync(entry1);
      }
      catch (NotSupportedException) { wasThrown = true; }
      Assert.IsTrue(wasThrown);
    }

    private PracticeHistoryEntriesController CreateController(DbContext context)
    {
      return new PracticeHistoryEntriesController(new DeleteValidatorBase<PracticeHistoryEntry>(context),
        new CommitValidatorBase<PracticeHistoryEntry>(context), context)
      { ControllerContext = ControllerContext };
    }
  }
}