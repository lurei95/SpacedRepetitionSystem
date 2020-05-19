using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Entities.Entities.Security;
using SpacedRepetitionSystem.WebAPI.Validation.CardTemplates;
using System;

namespace SpacedRepetitionSystem.WebApi.Tests.Validation.CardTemplates
{
  /// <summary>
  /// Testclass for <see cref="CardTemplateCommitValidatorTests"/>
  /// </summary>
  [TestClass]
  public sealed class CardTemplateCommitValidatorTests : EntityFrameWorkTestBase
  {
    /// <summary>
    /// Tests the validation of <see cref="CardTemplate.Title"/>
    /// </summary>
    [TestMethod]
    public void ValidatesTitleTest()
    {
      using DbContext context = CreateContext();
      CardTemplateCommitValidator validator = new CardTemplateCommitValidator(context);
      CardTemplate template = new CardTemplate() { CardTemplateId = 1 };

      //not successful
      string error = validator.Validate(template);
      Assert.IsFalse(string.IsNullOrEmpty(error));

      //successful
      template.Title = "Test";
      error = validator.Validate(template);
      Assert.IsTrue(string.IsNullOrEmpty(error));
    }

    /// <summary>
    /// Tests that the title of the template is unique
    /// </summary>
    [TestMethod]
    public void ValidatesTitleUniqueTest()
    {
      Guid userId = Guid.NewGuid();
      CreateData((context) =>
      {
        User user = new User() { UserId = userId };
        CardTemplate template1 = new CardTemplate()
        {
          UserId = userId,
          Title = "test1",
          CardTemplateId = 1
        };
        CardTemplate template2 = new CardTemplate()
        {
          UserId = Guid.NewGuid(),
          Title = "test2",
          CardTemplateId = 2
        };
        context.Add(user);
        context.Add(template1);
        context.Add(template2);
      });

      //not successful
      using DbContext context = CreateContext();
      CardTemplateCommitValidator validator = new CardTemplateCommitValidator(context);
      CardTemplate template = new CardTemplate()
      {
        UserId = userId,
        Title = "test1",
        CardTemplateId = 3
      };
      string error = validator.Validate(template);
      Assert.IsFalse(string.IsNullOrEmpty(error));

      //successful
      template = new CardTemplate()
      {
        UserId = userId,
        Title = "test2",
        CardTemplateId = 3
      }; 
      error = validator.Validate(template);
      Assert.IsTrue(string.IsNullOrEmpty(error));
    }
  }
}