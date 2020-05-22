using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Entities.Entities.Security;
using SpacedRepetitionSystem.Utility.Notification;
using SpacedRepetitionSystem.WebAPI.Controllers.Cards;
using SpacedRepetitionSystem.WebAPI.Validation.Core;
using SpacedRepetitionSystem.WebAPI.Validation.Decks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.WebApi.Tests.Controllers.Cards
{
  /// <summary>
  /// Testclass for <see cref="DecksController"/>
  /// </summary>
  [TestClass]
  public sealed class DecksControllerTests : ControllerTestBase
  {
    private static User otherUser;
    private static CardTemplate template;
    private static CardFieldDefinition fieldDefinition1;
    private static Deck deck;
    private static Deck otherUserDeck;
    private static Card card;
    private static CardField field1;
    private static CardField field2;

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
      template = new CardTemplate()
      {
        UserId = User.UserId,
        CardTemplateId = 1,
        Title = "Default"
      };
      fieldDefinition1 = new CardFieldDefinition()
      {
        FieldId = 1,
        CardTemplateId = template.CardTemplateId,
        FieldName = "Front"
      };
      template.FieldDefinitions.Add(fieldDefinition1);
      deck = new Deck()
      {
        DeckId = 1,
        Title = "Default",
        DefaultCardTemplateId = template.CardTemplateId,
        UserId = User.UserId
      };
      otherUserDeck = new Deck()
      {
        DeckId = 2,
        Title = "Test",
        DefaultCardTemplateId = template.CardTemplateId,
        UserId = otherUser.UserId
      };
      card = new Card()
      {
        CardId = 1,
        CardTemplateId = template.CardTemplateId,
        UserId = User.UserId,
      };
      field1 = new CardField()
      {
        FieldId = 1,
        CardId = card.CardId,
        CardTemplateId = template.CardTemplateId,
        FieldName = "Front",
        Value = "Field1"
      };
      field2 = new CardField()
      {
        CardId = card.CardId,
        CardTemplateId = template.CardTemplateId,
        FieldName = "Back",
      };
      card.Fields.Add(field1);
      card.Fields.Add(field2);
      deck.Cards.Add(card);
    }

    ///<inheritdoc/>
    [TestInitialize]
    public override void TestInitialize()
    {
      base.TestInitialize();
      CreateData(context =>
      {
        context.Add(otherUser);
        context.Add(template);
        context.Add(deck);
        context.Add(otherUserDeck);
      });
    }

    /// <summary>
    /// Tests <see cref="DecksController.GetAsync(long)"/>
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task GetDeckByIdTest()
    {
      using DbContext context = CreateContext();
      DecksController controller = CreateController(context);

      //get deck successfully
      ActionResult<Deck> result = await controller.GetAsync(1);
      Assert.IsNotNull(result.Value);
      Assert.AreEqual(1, result.Value.Cards.Count);
      Assert.AreEqual(card.CardId, result.Value.Cards[0].CardId);
      Assert.AreEqual(1, result.Value.Cards[0].Fields.Count);
      Assert.AreEqual(fieldDefinition1.FieldName, result.Value.Cards[0].Fields[0].CardFieldDefinition.FieldName);

      //Deck of other user -> unauthorized
      result = await controller.GetAsync(2);
      Assert.IsTrue(result.Result is UnauthorizedResult);

      //Deck does not exist -> not found
      result = await controller.GetAsync(3);
      Assert.IsTrue(result.Result is NotFoundResult);
    }

    /// <summary>
    /// Tests <see cref="DecksController.GetAsync(IDictionary{string, object})/>
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task GetDecksTest()
    {
      using DbContext context = CreateContext();
      DecksController controller = CreateController(context);
      Dictionary<string, object> parameters = new Dictionary<string, object>();
      
      //Get all decks of user
      ActionResult<List<Deck>> result = await controller.GetAsync(parameters);
      Assert.AreEqual(1, result.Value.Count);
      Assert.AreEqual(deck.DeckId, result.Value[0].DeckId);
      Assert.AreEqual(2, result.Value[0].CardCount);
      Assert.AreEqual(1, result.Value[0].DueCardCount); //Only fields with a value are counted

      //Get all pinned decks
      parameters.Add(nameof(Deck.IsPinned), true);
      result = await controller.GetAsync(parameters);
      Assert.AreEqual(0, result.Value.Count);
    }

    /// <summary>
    /// Tests <see cref="DecksController.PostAsync(Deck entity)/>
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task PostDeckTest()
    {
      using DbContext context = CreateContext();
      DecksController controller = CreateController(context);

      //null as parameter -> bad request
      IActionResult result = await controller.PostAsync(null);
      Assert.IsTrue(result is BadRequestResult);

      //Create new valid entity
      Deck deck1 = new Deck()
      {
        DeckId = 3,
        Title = "test123",
        DefaultCardTemplateId = template.CardTemplateId
      };
      result = await controller.PostAsync(deck1);
      Assert.IsTrue(result is OkResult);
      deck1 = context.Find<Deck>((long)3);
      Assert.IsNotNull(deck1);
      Assert.AreEqual(User.UserId, deck1.UserId);

      //Invalid deck is validated
      bool wasThrown = false;
      try
      {
        result = await controller.PostAsync(new Deck());
      }
      catch (NotifyException)
      {
        wasThrown = true;
      }
      Assert.IsTrue(wasThrown);
    }

    /// <summary>
    /// Tests <see cref="DecksController.DeleteAsync(Deck entity)/>
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task DeleteDeckTest()
    {
      using DbContext context = CreateContext();
      DecksController controller = CreateController(context);

      //null as parameter -> bad request
      IActionResult result = await controller.DeleteAsync(null);
      Assert.IsTrue(result is BadRequestResult);

      //deck does not exist -> not found
      result = await controller.DeleteAsync(new Deck());
      Assert.IsTrue(result is NotFoundResult);

      //delete exitsting entity
      result = await controller.DeleteAsync(deck);
      Assert.IsTrue(result is OkResult);
      Deck deck1 = context.Find<Deck>((long)1);
      Assert.IsNull(deck1);
    }

    /// <summary>
    /// Tests <see cref="DecksController.PutAsync(Deck entity)/>
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task PutDeckTest()
    {
      using DbContext context = CreateContext();
      DecksController controller = CreateController(context);

      //null as parameter -> bad request
      IActionResult result = await controller.PutAsync(null);
      Assert.IsTrue(result is BadRequestResult);

      //deck does not exist in db -> not found
      Deck newDeck = new Deck()
      {
        DeckId = 5,
        Title = "dfsdf",
        DefaultCardTemplateId = template.CardTemplateId
      };
      result = await controller.PutAsync(newDeck);
      Assert.IsTrue(result is NotFoundResult);

      //Save changed entity
      deck.Title = "New Title";
      result = await controller.PutAsync(deck);
      Assert.IsTrue(result is OkResult);
      Deck deck1 = context.Find<Deck>(deck.DeckId);
      Assert.AreEqual("New Title", deck1.Title);

      //Invalid deck is validated
      bool wasThrown = false;
      try
      {
        result = await controller.PostAsync(new Deck());
      }
      catch (NotifyException)
      {
        wasThrown = true;
      }
      Assert.IsTrue(wasThrown);
    }

    private DecksController CreateController(DbContext context)
    {
      return new DecksController(new DeleteValidatorBase<Deck>(context),
        new DeckCommitValidator(context), context)
      { ControllerContext = ControllerContext };
    }
  }
}