using SpacedRepetitionSystem.Entities.Entities.Security;
using SpacedRepetitionSystem.Entities.Validation.Core;
using SpacedRepetitionSystem.Utility.Extensions;

namespace SpacedRepetitionSystem.Entities.Validation.Users
{
  /// <summary>
  /// Validator for <see cref="User.Email"/>
  /// </summary>
  public sealed class UserEmailValidator : PropertyValidatorBase<User, string>
  {
    ///<inheritdoc/>
    public override string PropertyName => nameof(User.Email);

    ///<inheritdoc/>
    public override string Validate(User entity, string newValue)
    {
      if (string.IsNullOrEmpty(newValue))
        return Errors.PropertyRequired.FormatWith(PropertyNames.EMailAddress);
      return null;
    }
  }
}