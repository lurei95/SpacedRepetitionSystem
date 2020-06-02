using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Entities.Entities.Security;
using SpacedRepetitionSystem.Utility.Notification;
using SpacedRepetitionSystem.WebAPI.Controllers.Cards;
using SpacedRepetitionSystem.WebAPI.Core;
using SpacedRepetitionSystem.WebAPI.Validation.Cards;
using SpacedRepetitionSystem.WebAPI.Validation.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.WebApi.Tests.Controllers.Cards
{
  /// <summary>
  /// Testclass for <see cref="CardsController"/>
  /// </summary>
  [TestClass]
  public sealed class CardsControllerTests : ControllerTestBase
  {
    private static User otherUser;
    private static CardTemplate template;
    private static CardFieldDefinition fieldDefinition1;
    private static CardFieldDefinition fieldDefinition2;
    private static Deck deck;
    private static Card card;
    private static Card otherUserCard;
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
      fieldDefinition2 = new CardFieldDefinition()
      {
        FieldId = 2,
        CardTemplateId = template.CardTemplateId,
        FieldName = "Back"
      };
      template.FieldDefinitions.Add(fieldDefinition1);
      template.FieldDefinitions.Add(fieldDefinition2);

      deck = new Deck()
      {
        DeckId = 1,
        Title = "Default",
        DefaultCardTemplateId = template.CardTemplateId,
        UserId = User.UserId
      };
      card = new Card()
      {
        CardId = 1,
        CardTemplateId = template.CardTemplateId,
        UserId = User.UserId,
      };
      otherUserCard = new Card()
      {
        CardId = 2,
        CardTemplateId = template.CardTemplateId,
        UserId = otherUser.UserId,
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
        FieldId = 2,
        CardId = card.CardId,
        CardTemplateId = template.CardTemplateId,
        FieldName = "Back",
        Value = "Field2"
      };
      card.Fields.Add(field1);
      card.Fields.Add(field2);
      deck.Cards.Add(card);
      deck.Cards.Add(otherUserCard);
    }

    ///<inheritdoc/>
    [TestInitialize]
    public override void TestInitialize()
    {
      base.TestInitialize();
      CreateData(context =>
      {
        context.Add(otherUser);
        context.Add(User);
        context.Add(template);
        context.Add(deck);
      });
    }

    /// <summary>
    /// Tests <see cref="CardsController.GetAsync(long)"/>
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task GetCardByIdTest()
    {
      using DbContext context = CreateContext();
      CardsController controller = CreateController(context);

      //get card successfully
      ActionResult<Card> result = await controller.GetAsync(1);
      Assert.IsNotNull(result.Value);
      Assert.AreEqual(deck.DeckId, result.Value.Deck.DeckId);
      Assert.AreEqual(template.CardTemplateId, result.Value.CardTemplate.CardTemplateId);
      Assert.AreEqual(field1.FieldName, result.Value.Fields[0].FieldName);
      Assert.AreEqual(field2.FieldName, result.Value.Fields[1].FieldName);

      //Card of other user -> unauthorized
      result = await controller.GetAsync(2);
      Assert.IsTrue(result.Result is UnauthorizedResult);

      //Card does not exist -> not found
      result = await controller.GetAsync(3);
      Assert.IsTrue(result.Result is NotFoundResult);
    }

    /// <summary>
    /// Tests <see cref="CardsController.GetAsync(IDictionary{string, object})/>
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task GetCardsTest()
    {
      using DbContext context = CreateContext();
      CardsController controller = CreateController(context);
      Dictionary<string, object> parameters = new Dictionary<string, object>();
      
      //Get all cards of user
      ActionResult<List<Card>> result = await controller.GetAsync(parameters);
      Assert.AreEqual(1, result.Value.Count);
      Assert.AreEqual(card.CardId, result.Value[0].CardId);
      Assert.AreEqual(deck.DeckId, result.Value[0].Deck.DeckId);
      Assert.AreEqual(template.CardTemplateId, result.Value[0].CardTemplate.CardTemplateId);
      Assert.AreEqual(field1.FieldName, result.Value[0].Fields[0].FieldName);
      Assert.AreEqual(field2.FieldName, result.Value[0].Fields[1].FieldName);

      //Get all cards of deck
      parameters.Add(nameof(Deck.DeckId), deck.DeckId);
      result = await controller.GetAsync(parameters);
      Assert.AreEqual(1, result.Value.Count);
      Assert.AreEqual(card.CardId, result.Value[0].CardId);
      Assert.AreEqual(deck.DeckId, result.Value[0].Deck.DeckId);
      Assert.AreEqual(template.CardTemplateId, result.Value[0].CardTemplate.CardTemplateId);
      Assert.AreEqual(field1.FieldName, result.Value[0].Fields[0].FieldName);
      Assert.AreEqual(field2.FieldName, result.Value[0].Fields[1].FieldName);

      //Get all cards of not existing deck
      parameters[nameof(Deck.DeckId)] = (long)2;
      result = await controller.GetAsync(parameters);
      Assert.AreEqual(0, result.Value.Count);
    }

    /// <summary>
    /// Tests <see cref="CardsController.GetAsync(IDictionary{string, object})/>
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task GetCardsWithSearchTextTest()
    {
      using DbContext context = CreateContext();
      CardsController controller = CreateController(context);
      Dictionary<string, object> parameters = new Dictionary<string, object>()
      { { EntityControllerBase<Card, long>.SearchTextParameter, "test123" } };

      //No cards matching the search text
      ActionResult<List<Card>> result = await controller.GetAsync(parameters);
      Assert.AreEqual(0, result.Value.Count);

      //Search text is card id
      parameters[EntityControllerBase<Card, long>.SearchTextParameter] = "1";
      result = await controller.GetAsync(parameters);
      Assert.AreEqual(1, result.Value.Count);
      Assert.AreEqual(card.CardId, result.Value[0].CardId);

      //Deck title contains the search text
      parameters[EntityControllerBase<Card, long>.SearchTextParameter] = "fault";
      result = await controller.GetAsync(parameters);
      Assert.AreEqual(1, result.Value.Count);
      Assert.AreEqual(card.CardId, result.Value[0].CardId);

      //One of the field names contains the search text
      parameters[EntityControllerBase<Card, long>.SearchTextParameter] = "Fro";
      result = await controller.GetAsync(parameters);
      Assert.AreEqual(1, result.Value.Count);
      Assert.AreEqual(card.CardId, result.Value[0].CardId);
    }

    /// <summary>
    /// Tests <see cref="CardsController.PostAsync(Card entity)/>
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task PostCardTest()
    {
      using DbContext context = CreateContext();
      CardsController controller = CreateController(context);

      //null as parameter -> bad request
      ActionResult<Card> result = await controller.PostAsync(null);
      Assert.IsTrue(result.Result is BadRequestResult);

      //Create new valid entity
      Card card1 = new Card()
      {
        CardId = 3,
        DeckId = deck.DeckId,
        CardTemplateId = template.CardTemplateId
      };
      card1.Fields.Add(new CardField() { FieldId = 1, FieldName = "test1", Value = "test1" });
      card1.Fields.Add(new CardField() { FieldId = 2, FieldName = "test2", Value = "test2" });
      result = await controller.PostAsync(card1);
      Assert.IsNotNull(result.Value);
      card1 = context.Find<Card>((long)3);
      Assert.IsNotNull(card1);
      Assert.AreEqual(User.UserId, card1.UserId);

      //Invalid card is validated
      bool wasThrown = false;
      try
      {
        result = await controller.PostAsync(new Card());
      }
      catch (NotifyException)
      {
        wasThrown = true;
      }
      Assert.IsTrue(wasThrown);
    }

    /// <summary>
    /// Tests <see cref="CardsController.DeleteAsync(Card entity)/>
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task DeleteCardTest()
    {
      using DbContext context = CreateContext();
      CardsController controller = CreateController(context); ;

      //null as parameter -> bad request
      IActionResult result = await controller.DeleteAsync(null);
      Assert.IsTrue(result is BadRequestResult);

      //card does not exist -> not found
      result = await controller.DeleteAsync(new Card());
      Assert.IsTrue(result is NotFoundResult);

      //delete exitsting entity
      result = await controller.DeleteAsync(card);
      Assert.IsTrue(result is OkResult);
      Card card1 = context.Find<Card>((long)1);
      Assert.IsNull(card1);
    }

    /// <summary>
    /// Tests <see cref="CardsController.PutAsync(Card entity)/>
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task PutCardTest()
    {
      using DbContext context = CreateContext();
      CardsController controller = CreateController(context);

      //null as parameter -> bad request
      ActionResult<Card> result = await controller.PutAsync(null);
      Assert.IsTrue(result.Result is BadRequestResult);

      //card does not exist in db -> not found
      Card newCard = new Card()
      {
        CardId = 5,
        DeckId = deck.DeckId,
        CardTemplateId = template.CardTemplateId
      };
      newCard.Fields.Add(new CardField() { FieldId = 1, FieldName = "test1", Value = "test1" });
      newCard.Fields.Add(new CardField() { FieldId = 2, FieldName = "test2", Value = "test2" });
      result = await controller.PutAsync(newCard);
      Assert.IsTrue(result.Result is NotFoundResult);

      //Save changed entity
      card.Tags = "test";
      result = await controller.PutAsync(card);
      Assert.IsNotNull(result.Value);
      Card card1 = context.Find<Card>(card.CardId);
      Assert.AreEqual("test", card1.Tags);
      Assert.AreEqual(2, card1.Fields.Count);
      Assert.AreEqual(field1.FieldName, card1.Fields[0].FieldName);

      //Invalid card is validated
      bool wasThrown = false;
      try
      {
        result = await controller.PostAsync(new Card());
      }
      catch (NotifyException)
      {
        wasThrown = true;
      }
      Assert.IsTrue(wasThrown);
    }

    private CardsController CreateController(DbContext context)
    {
      return new CardsController(new DeleteValidatorBase<Card>(context),
        new CardCommitValidator(context), context)
      { ControllerContext = ControllerContext };
    }
  }
}