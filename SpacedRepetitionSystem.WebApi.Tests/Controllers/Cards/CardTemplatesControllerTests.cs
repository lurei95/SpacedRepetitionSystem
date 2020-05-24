using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Entities.Entities.Security;
using SpacedRepetitionSystem.Utility.Notification;
using SpacedRepetitionSystem.WebAPI.Controllers.Cards;
using SpacedRepetitionSystem.WebAPI.Core;
using SpacedRepetitionSystem.WebAPI.Validation.CardTemplates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.WebApi.Tests.Controllers.Cards
{
  /// <summary>
  /// Testclass for <see cref="CardTemplatesController"/>
  /// </summary>
  [TestClass]
  public sealed class CardTemplatesControllerTests : ControllerTestBase
  {
    private static User otherUser;
    private static CardTemplate template1;
    private static CardTemplate template2;
    private static CardTemplate otherUserTemplate;
    private static CardFieldDefinition fieldDefinition1;
    private static CardFieldDefinition fieldDefinition2;
    private static Card card;
    private static CardField field; 

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
      template1 = new CardTemplate()
      {
        UserId = User.UserId,
        CardTemplateId = 1,
        Title = "Default"
      };
      template2 = new CardTemplate()
      {
        UserId = User.UserId,
        CardTemplateId = 3,
        Title = "asdefaxyfsdfult"
      };
      otherUserTemplate = new CardTemplate()
      {
        UserId = otherUser.UserId,
        CardTemplateId = 2,
        Title = "Default"
      };
      fieldDefinition1 = new CardFieldDefinition()
      {
        FieldId = 1,
        CardTemplateId = template1.CardTemplateId,
        FieldName = "Front"
      };
      fieldDefinition2 = new CardFieldDefinition()
      {
        FieldId = 1,
        CardTemplateId = template2.CardTemplateId,
        FieldName = "Back"
      };
      card = new Card() 
      { 
        CardId = 1,
        CardTemplateId = template1.CardTemplateId 
      };
      field = new CardField()
      {
        CardId = card.CardId,
        CardTemplateId = template1.CardTemplateId,
        FieldName = "Front",
        FieldId = 1,
      };
      template1.FieldDefinitions.Add(fieldDefinition1);
      template2.FieldDefinitions.Add(fieldDefinition2);
      card.Fields.Add(field);
    }

    ///<inheritdoc/>
    [TestInitialize]
    public override void TestInitialize()
    {
      base.TestInitialize();
      CreateData(context =>
      {
        context.Add(otherUser);
        context.Add(template1);
        context.Add(template2);
        context.Add(otherUserTemplate);
        context.Add(field);
        context.Add(card);
      });
    }

    /// <summary>
    /// Tests <see cref="CardTemplatesController.GetAsync(long)"/>
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task GetTemplateByIdTest()
    {
      using DbContext context = CreateContext();
      CardTemplatesController controller = CreateController(context);

      //get template successfully
      ActionResult<CardTemplate> result = await controller.GetAsync(3);
      Assert.IsNotNull(result.Value);
      Assert.AreEqual(1, result.Value.FieldDefinitions.Count);

      //template of other user -> unauthorized
      result = await controller.GetAsync(2);
      Assert.IsTrue(result.Result is UnauthorizedResult);

      //template does not exist -> not found
      result = await controller.GetAsync(4);
      Assert.IsTrue(result.Result is NotFoundResult);
    }

    /// <summary>
    /// Tests <see cref="CardTemplatesController.GetAsync(IDictionary{string, object})/>
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task GetCardTemplatesTest()
    {
      using DbContext context = CreateContext();
      CardTemplatesController controller = CreateController(context);
      Dictionary<string, object> parameters = new Dictionary<string, object>();
      
      //Get all templates of user
      ActionResult<List<CardTemplate>> result = await controller.GetAsync(parameters);
      Assert.AreEqual(2, result.Value.Count);
      Assert.AreEqual(1, result.Value[1].FieldDefinitions.Count);
    }

    /// <summary>
    /// Tests <see cref="CardTemplatesController.PostAsync(CardTemplate entity)/>
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task PostCardTemplateTest()
    {
      using DbContext context = CreateContext();
      CardTemplatesController controller = CreateController(context);

      //null as parameter -> bad request
      IActionResult result = await controller.PostAsync(null);
      Assert.IsTrue(result is BadRequestResult);

      //Create new valid entity
      CardTemplate cardTemplate1 = new CardTemplate()
      {
        CardTemplateId = 4,
        Title = "test123"
      };
      result = await controller.PostAsync(cardTemplate1);
      Assert.IsTrue(result is OkResult);
      cardTemplate1 = context.Find<CardTemplate>((long)3);
      Assert.IsNotNull(cardTemplate1);
      Assert.AreEqual(User.UserId, cardTemplate1.UserId);

      //Invalid template is validated
      bool wasThrown = false;
      try
      {
        result = await controller.PostAsync(new CardTemplate());
      }
      catch (NotifyException)
      {
        wasThrown = true;
      }
      Assert.IsTrue(wasThrown);
    }

    /// <summary>
    /// Tests <see cref="CardTemplatesController.DeleteAsync(CardTemplate entity)/>
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task DeleteCardTemplateTest()
    {
      using DbContext context = CreateContext();
      CardTemplatesController controller = CreateController(context);

      //null as parameter -> bad request
      IActionResult result = await controller.DeleteAsync(null);
      Assert.IsTrue(result is BadRequestResult);

      //template does not exist -> not found
      result = await controller.DeleteAsync(new CardTemplate());
      Assert.IsTrue(result is NotFoundResult);

      //delete existing entity
      result = await controller.DeleteAsync(template2);
      Assert.IsTrue(result is OkResult);
      CardTemplate template1 = context.Find<CardTemplate>((long)3);
      Assert.IsNull(template1);
    }

    /// <summary>
    /// Tests <see cref="CardTemplatesController.PutAsync(CardTemplate entity)/>
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task PutCardTemplateTest()
    {
      using DbContext context = CreateContext();
      CardTemplatesController controller = CreateController(context);

      //null as parameter -> bad request
      IActionResult result = await controller.PutAsync(null);
      Assert.IsTrue(result is BadRequestResult);

      //template does not exist in db -> not found
      CardTemplate newTemplate = new CardTemplate()
      {
        CardTemplateId = 5,
        Title = "dfsdf"
      };
      result = await controller.PutAsync(newTemplate);
      Assert.IsTrue(result is NotFoundResult);

      //Save changed entity
      template2.Title = "New Title";
      result = await controller.PutAsync(template2);
      Assert.IsTrue(result is OkResult);
      CardTemplate template3 = context.Find<CardTemplate>(template2.CardTemplateId);
      Assert.AreEqual("New Title", template3.Title);
      Assert.AreEqual(1, template3.FieldDefinitions.Count);

      //Invalid template is validated
      bool wasThrown = false;
      try
      {
        result = await controller.PostAsync(new CardTemplate());
      }
      catch (NotifyException)
      {
        wasThrown = true;
      }
      Assert.IsTrue(wasThrown);
    }

    /// <summary>
    /// Tests <see cref="CardTemplatesController.PutAsync(CardTemplate entity)/>
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task UpdateFieldDefinitionOnPutCardTemplateTest()
    {
      using DbContext context = CreateContext();
      CardTemplatesController controller = CreateController(context);

      //Save changed entity
      template1.FieldDefinitions[0].FieldName = "NewFieldName";
      template1.FieldDefinitions[0].ShowInputForPractice = true;
      IActionResult result = await controller.PutAsync(template1);
      Assert.IsTrue(result is OkResult);
      CardFieldDefinition definition3 = context.Find<CardFieldDefinition>(template1.CardTemplateId, 1);
      Assert.IsTrue(definition3.ShowInputForPractice);
      Assert.AreEqual("NewFieldName", definition3.FieldName);
      Card card1 = context.Set<Card>().SingleOrDefault(card => card.CardId == 1);
      Assert.AreEqual(1, card1.Fields.Count);
      Assert.AreEqual("NewFieldName", card1.Fields[0].FieldName);
    }

    /// <summary>
    /// Tests <see cref="CardTemplatesController.GetAsync(IDictionary{string, object})/>
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task GetCardTemplatesWithSearchTextTest()
    {
      using DbContext context = CreateContext();
      CardTemplatesController controller = CreateController(context);
      Dictionary<string, object> parameters = new Dictionary<string, object>()
      { { EntityControllerBase<CardTemplate, long>.SearchTextParameter, "test123" } };

      //No cards matching the search text
      ActionResult<List<CardTemplate>> result = await controller.GetAsync(parameters);
      Assert.AreEqual(0, result.Value.Count);

      //Search text is template id
      parameters[EntityControllerBase<Card, long>.SearchTextParameter] = "1";
      result = await controller.GetAsync(parameters);
      Assert.AreEqual(1, result.Value.Count);
      Assert.AreEqual(template1.CardTemplateId, result.Value[0].CardTemplateId);

      //title contains the search text
      parameters[EntityControllerBase<Card, long>.SearchTextParameter] = "fault";
      result = await controller.GetAsync(parameters);
      Assert.AreEqual(1, result.Value.Count);
      Assert.AreEqual(template1.CardTemplateId, result.Value[0].CardTemplateId);

      //One of the field names contains the search text
      parameters[EntityControllerBase<Card, long>.SearchTextParameter] = "Fro";
      result = await controller.GetAsync(parameters);
      Assert.AreEqual(1, result.Value.Count);
      Assert.AreEqual(template1.CardTemplateId, result.Value[0].CardTemplateId);
    }

    /// <summary>
    /// Tests <see cref="CardTemplatesController.PutAsync(CardTemplate entity)/>
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task RemoveFieldDefinitionOnPutCardTemplateTest()
    {
      using DbContext context = CreateContext();
      CardTemplatesController controller = CreateController(context);

      //Save changed entity
      template1.FieldDefinitions.Clear();
      IActionResult result = await controller.PutAsync(template1);
      Assert.IsTrue(result is OkResult);
      CardTemplate template3 = context.Find<CardTemplate>((long)1);
      Assert.AreEqual(0, template3.FieldDefinitions.Count);
      Card card1 = context.Find<Card>((long)1);
      Assert.AreEqual(0, card1.Fields.Count);
    }

    /// <summary>
    /// Tests <see cref="CardTemplatesController.PutAsync(CardTemplate entity)/>
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task AddNewFieldDefinitionOnPutCardTemplateTest()
    {
      using DbContext context = CreateContext();
      CardTemplatesController controller = CreateController(context);

      //Save changed entity
      CardFieldDefinition fieldDefinition2 = new CardFieldDefinition()
      {
        CardTemplateId = 1,
        FieldName = "Field2",
      };
      template1.FieldDefinitions.Add(fieldDefinition2);
      IActionResult result = await controller.PutAsync(template1);
      Assert.IsTrue(result is OkResult);
      CardTemplate template3 = context.Set<CardTemplate>().FirstOrDefault(template => template.CardTemplateId == 1);
      Assert.AreEqual(2, template3.FieldDefinitions.Count);
      Assert.AreEqual(2, template3.FieldDefinitions[1].FieldId);
      Card card1 = context.Set<Card>().Include(card => card.Fields).FirstOrDefault(card => card.CardId == 1);
      Assert.AreEqual(2, card1.Fields.Count);
      Assert.AreEqual("Field2", card1.Fields.OrderBy(field => field.FieldId).ToList()[1].FieldName);
    }

    private CardTemplatesController CreateController(DbContext context)
    {
      return new CardTemplatesController(new CardTemplateDeleteValidator(context),
        new CardTemplateCommitValidator(context), context)
      { ControllerContext = ControllerContext };
    }
  }
}