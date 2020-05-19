using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpacedRepetitionSystem.Entities.Entities.Security;
using SpacedRepetitionSystem.WebAPI.Validation.Cards;
using System;

namespace SpacedRepetitionSystem.WebApi.Tests.Validation.Users
{
  /// <summary>
  /// Testclass for <see cref="UserCommitValidator"/>
  /// </summary>
  [TestClass]
  public sealed class UserCommitValidatorTests : EntityFrameWorkTestBase
  {
    /// <summary>
    /// Tests that the email of a user must be unique
    /// </summary>
    [TestMethod]
    public void ValidatesEmailIsUniqueTest()
    {
      CreateData((context) =>
      {
        User user = new User() 
        { 
          UserId = Guid.NewGuid(), 
          Email = "test" 
        };
        context.Add(user);
      });

      //not successful
      using DbContext context = CreateContext();
      UserCommitValidator validator = new UserCommitValidator(context);
      User user = new User()
      {
        UserId = Guid.NewGuid(),
        Email = "test"
      };
      string error = validator.Validate(user);
      Assert.IsFalse(string.IsNullOrEmpty(error));

      //successful
      user = new User()
      {
        UserId = Guid.NewGuid(),
        Email = "test1"
      };
      error = validator.Validate(user);
      Assert.IsTrue(string.IsNullOrEmpty(error));
    }
  }
}