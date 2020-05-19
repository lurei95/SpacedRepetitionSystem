using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpacedRepetitionSystem.Entities.Entities.Security;
using SpacedRepetitionSystem.Entities.Validation.Users;

namespace SpacedRepetitionSystem.Entities.Tests.Validation.CardTemplates
{
  /// <summary>
  /// Testclass for <see cref="UserPasswordValidator"/>
  /// </summary>
  [TestClass]
  public sealed class UserPasswordValidatorTests
  {
    /// <summary>
    /// Tests that <see cref="User.Password"/> is validated
    /// </summary>
    [TestMethod]
    public void ValidateTest()
    {
      UserPasswordValidator validator = new UserPasswordValidator();
      string error = validator.Validate(new User(), null);
      Assert.IsFalse(string.IsNullOrEmpty(error));
      error = validator.Validate(new User(), "");
      Assert.IsFalse(string.IsNullOrEmpty(error));
      error = validator.Validate(new User(), "test");
      Assert.IsTrue(string.IsNullOrEmpty(error));
    }
  }
}