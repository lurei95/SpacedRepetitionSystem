using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Entities.Entities.Security;
using SpacedRepetitionSystem.Entities.Validation.Core;
using SpacedRepetitionSystem.WebAPI.Controllers.Cards;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.WebApi.Tests.Controllers.Cards
{
  /// <summary>
  /// Testclass for <see cref="PracticeHistoryEntriesController"/>
  /// </summary>
  [TestClass]
  public sealed class PracticeHistoryEntriesControllerTests : EntityFrameWorkTestCore
  {
    private static User otherUser;
    private static User user;
    private static CardField field;
    private static PracticeHistoryEntry entry1;
    private static PracticeHistoryEntry entry2;
    private static PracticeHistoryEntry entry3;
    private static PracticeHistoryEntry otherUserEntry;
    private static ControllerContext controllerContext;

    /// <summary>
    /// Creates the test data when the class is initialized
    /// </summary>
    /// <param name="context">TestContext</param>
    [ClassInitialize]
#pragma warning disable IDE0060 // Remove unused parameter
    public static void ClassInitialize(TestContext context)
#pragma warning restore IDE0060 // Remove unused parameter
    {
      user = new User()
      {
        UserId = Guid.NewGuid(),
        Email = "test@test.com",
        Password = "test"
      };
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
        FieldName = "Field 1",
        PracticeDate = DateTime.Today,
        PracticeHistoryEntryId = 1,
        UserId = user.UserId,
        CorrectCount = 1,
        HardCount = 2,
        WrongCount = 3
      };
      entry2 = new PracticeHistoryEntry()
      {
        CardId = 2,
        DeckId = 2,
        FieldName = "Field 1",
        PracticeDate = DateTime.Today,
        PracticeHistoryEntryId = 2,
        UserId = user.UserId
      };
      entry3 = new PracticeHistoryEntry()
      {
        CardId = 1,
        DeckId = 1,
        FieldName = "Field 3",
        PracticeDate = DateTime.Today,
        PracticeHistoryEntryId = 3,
        UserId = user.UserId,
        CorrectCount = 2,
        HardCount = 4,
        WrongCount = 6
      };
      otherUserEntry = new PracticeHistoryEntry()
      {
        CardId = 1,
        DeckId = 1,
        FieldName = "Field 2",
        PracticeDate = DateTime.Today,
        PracticeHistoryEntryId = 4,
        UserId = otherUser.UserId
      };
      field = new CardField()
      {
        CardId = 1,
        FieldName = "Field 1",
        ProficiencyLevel = 3
      };

      var identity = new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, Convert.ToString(user.UserId)) }, "TestAuthType");
      var claimsPrincipal = new ClaimsPrincipal(identity);
      controllerContext = new ControllerContext
      { HttpContext = new DefaultHttpContext { User = claimsPrincipal } };
    }

    ///<inheritdoc/>
    [TestInitialize]
    public override void TestInitialize()
    {
      base.TestInitialize();
      CreateData(context =>
      {
        context.Add(otherUser);
        context.Add(user);
        context.Add(field);
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
      parameters.Add(nameof(CardField.FieldName), "Field 3");
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
      IActionResult result = await controller.PostAsync(null);
      Assert.IsTrue(result is BadRequestResult);

      //Same day -> update existing
      PracticeHistoryEntry newEntry = new PracticeHistoryEntry()
      {
        CardId = 1,
        DeckId = 1,
        FieldName = "Field 1",
        PracticeDate = DateTime.Today,
        PracticeHistoryEntryId = 5,
        UserId = user.UserId,
        CorrectCount = 0,
        HardCount = 0,
        WrongCount = 1
      };
      result = await controller.PostAsync(newEntry);
      Assert.IsTrue(result is OkResult);
      newEntry = context.Find<PracticeHistoryEntry>((long)5);
      Assert.IsNull(newEntry);
      newEntry = context.Find<PracticeHistoryEntry>((long)1);
      Assert.AreEqual(4, newEntry.WrongCount);
      CardField field1 = context.Find<CardField>((long)1, "Field 1");
      Assert.AreEqual(1, field1.ProficiencyLevel);
      Assert.AreEqual(DateTime.Today.AddDays(1).Date, field1.DueDate.Date);

      //other day -> new entry
      newEntry = new PracticeHistoryEntry()
      {
        CardId = 1,
        DeckId = 1,
        FieldName = "Field 1",
        PracticeDate = DateTime.Today.AddDays(1),
        PracticeHistoryEntryId = 5,
        UserId = user.UserId,
        CorrectCount = 1,
        HardCount = 0,
        WrongCount = 0
      };
      result = await controller.PostAsync(newEntry);
      Assert.IsTrue(result is OkResult);
      newEntry = context.Find<PracticeHistoryEntry>((long)5);
      Assert.AreEqual(1, newEntry.CorrectCount);
      field1 = context.Find<CardField>((long)1, "Field 1");
      Assert.AreEqual(2, field1.ProficiencyLevel);
      Assert.AreEqual(DateTime.Today.AddDays(3).Date, field1.DueDate.Date);

      //other day -> new entry
      newEntry = new PracticeHistoryEntry()
      {
        CardId = 1,
        DeckId = 1,
        FieldName = "Field 1",
        PracticeDate = DateTime.Today.AddDays(1),
        PracticeHistoryEntryId = 6,
        UserId = user.UserId,
        CorrectCount = 0,
        HardCount = 1,
        WrongCount = 0
      };
      result = await controller.PostAsync(newEntry);
      Assert.IsTrue(result is OkResult);
      field1 = context.Find<CardField>((long)1, "Field 1");
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
        IActionResult result = await controller.PutAsync(entry1);
      }
      catch (NotSupportedException) { wasThrown = true; }
      Assert.IsTrue(wasThrown);
    }

    private PracticeHistoryEntriesController CreateController(DbContext context)
    {
      return new PracticeHistoryEntriesController(new DeleteValidatorBase<PracticeHistoryEntry>(context),
        new CommitValidatorBase<PracticeHistoryEntry>(context), context)
      { ControllerContext = controllerContext };
    }
  }
}