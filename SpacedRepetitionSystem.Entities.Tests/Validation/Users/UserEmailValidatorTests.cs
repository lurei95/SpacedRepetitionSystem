using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpacedRepetitionSystem.Entities.Entities.Security;
using SpacedRepetitionSystem.Entities.Validation.Users;

namespace SpacedRepetitionSystem.Entities.Tests.Validation.CardTemplates
{
  /// <summary>
  /// Testclass for <see cref="UserEmailValidator"/>
  /// </summary>
  [TestClass]
  public sealed class UserEmailValidatorTests
  {
    /// <summary>
    /// Tests that <see cref="User.Email"/> is validated
    /// </summary>
    [TestMethod]
    public void ValidateTest()
    {
      UserEmailValidator validator = new UserEmailValidator();
      string error = validator.Validate(new User(), null);
      Assert.IsFalse(string.IsNullOrEmpty(error));
      error = validator.Validate(new User(), "");
      Assert.IsFalse(string.IsNullOrEmpty(error));
      error = validator.Validate(new User(), "test");
      Assert.IsTrue(string.IsNullOrEmpty(error));
    }
  }
}