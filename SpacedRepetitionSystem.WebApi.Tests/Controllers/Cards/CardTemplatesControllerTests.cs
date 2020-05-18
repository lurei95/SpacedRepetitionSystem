using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Entities.Entities.Security;
using SpacedRepetitionSystem.Entities.Validation.CardTemplates;
using SpacedRepetitionSystem.Utility.Notification;
using SpacedRepetitionSystem.WebAPI.Controllers.Cards;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.WebApi.Tests.Controllers.Cards
{
  /// <summary>
  /// Testclass for <see cref="CardTemplatesController"/>
  /// </summary>
  [TestClass]
  public sealed class CardTemplatesControllerTests : EntityFrameWorkTestCore
  {
    private static User otherUser;
    private static User user;
    private static CardTemplate template;
    private static CardTemplate otherUserTemplate;
    private static CardFieldDefinition fieldDefinition1;
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

      template = new CardTemplate()
      {
        UserId = user.UserId,
        CardTemplateId = 1,
        Title = "Default"
      };
      otherUserTemplate = new CardTemplate()
      {
        UserId = otherUser.UserId,
        CardTemplateId = 2,
        Title = "Default"
      };
      fieldDefinition1 = new CardFieldDefinition()
      {
        CardTemplateId = template.CardTemplateId,
        FieldName = "Front"
      };
      template.FieldDefinitions.Add(fieldDefinition1);

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
        context.Add(template);
        context.Add(otherUserTemplate);
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
      CardTemplatesController controller = new CardTemplatesController(new CardTemplateDeleteValidator(context),
        new CardTemplateCommitValidator(context), context)
      { ControllerContext = controllerContext };

      //get template successfully
      ActionResult<CardTemplate> result = await controller.GetAsync(1);
      Assert.IsNotNull(result.Value);
      Assert.AreEqual(1, result.Value.FieldDefinitions.Count);

      //template of other user -> unauthorized
      result = await controller.GetAsync(2);
      Assert.IsTrue(result.Result is UnauthorizedResult);

      //template does not exist -> not found
      result = await controller.GetAsync(3);
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
      CardTemplatesController controller = new CardTemplatesController(new CardTemplateDeleteValidator(context),
        new CardTemplateCommitValidator(context), context)
      { ControllerContext = controllerContext };
      Dictionary<string, object> parameters = new Dictionary<string, object>();
      
      //Get all templates of user
      ActionResult<List<CardTemplate>> result = await controller.GetAsync(parameters);
      Assert.AreEqual(1, result.Value.Count);
      Assert.AreEqual(1, result.Value[0].FieldDefinitions.Count);
    }

    /// <summary>
    /// Tests <see cref="CardTemplatesController.PostAsync(CardTemplate entity)/>
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task PostCardTemplateTest()
    {
      using DbContext context = CreateContext();
      CardTemplatesController controller = new CardTemplatesController(new CardTemplateDeleteValidator(context),
        new CardTemplateCommitValidator(context), context)
      { ControllerContext = controllerContext };

      //null as parameter -> bad request
      IActionResult result = await controller.PostAsync(null);
      Assert.IsTrue(result is BadRequestResult);

      //Create new valid entity
      CardTemplate cardTemplate1 = new CardTemplate()
      {
        CardTemplateId = 3,
        Title = "test123"
      };
      result = await controller.PostAsync(cardTemplate1);
      Assert.IsTrue(result is OkResult);
      cardTemplate1 = context.Find<CardTemplate>((long)3);
      Assert.IsNotNull(cardTemplate1);
      Assert.AreEqual(user.UserId, cardTemplate1.UserId);

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
    public async Task DeleteDeckTest()
    {
      using DbContext context = CreateContext();
      CardTemplatesController controller = new CardTemplatesController(new CardTemplateDeleteValidator(context),
        new CardTemplateCommitValidator(context), context)
      { ControllerContext = controllerContext };

      //null as parameter -> bad request
      IActionResult result = await controller.DeleteAsync(null);
      Assert.IsTrue(result is BadRequestResult);

      //template does not exist -> not found
      result = await controller.DeleteAsync(new CardTemplate());
      Assert.IsTrue(result is NotFoundResult);

      //delete exitsting entity
      result = await controller.DeleteAsync(template);
      Assert.IsTrue(result is OkResult);
      CardTemplate template1 = context.Find<CardTemplate>((long)1);
      Assert.IsNull(template1);
    }

    /// <summary>
    /// Tests <see cref="CardTemplatesController.PutAsync(CardTemplate entity)/>
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task PutCardTest()
    {
      using DbContext context = CreateContext();
      CardTemplatesController controller = new CardTemplatesController(new CardTemplateDeleteValidator(context),
        new CardTemplateCommitValidator(context), context)
      { ControllerContext = controllerContext };

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
      template.Title = "New Title";
      template.FieldDefinitions.Remove(fieldDefinition1);
      CardFieldDefinition newField = new CardFieldDefinition()
      {
        CardTemplateId = template.CardTemplateId,
        FieldName = "NewField"
      };
      template.FieldDefinitions.Add(newField);
      result = await controller.PutAsync(template);
      Assert.IsTrue(result is OkResult);
      CardTemplate template1 = context.Find<CardTemplate>(template.CardTemplateId);
      Assert.AreEqual("New Title", template1.Title);
      Assert.AreEqual(1, template1.FieldDefinitions.Count);
      Assert.AreEqual(newField.FieldName, template1.FieldDefinitions[0].FieldName);

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
  }
}