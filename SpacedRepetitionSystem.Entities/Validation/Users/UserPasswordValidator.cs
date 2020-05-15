using SpacedRepetitionSystem.Entities.Entities.Users;
using SpacedRepetitionSystem.Entities.Validation.Core;
using SpacedRepetitionSystem.Utility.Extensions;

namespace SpacedRepetitionSystem.Entities.Validation.Users
{
  /// <summary>
  /// Validator for <see cref="User.UserId"/>
  /// </summary>
  public sealed class UserUserIdValidator : PropertyValidatorBase<User, string>
  {
    ///<inheritdoc/>
    public override string PropertyName => nameof(User.UserId);

    ///<inheritdoc/>
    public override string Validate(User entity, string newValue)
    {
      if (string.IsNullOrEmpty(newValue))
        return Errors.PropertyRequired.FormatWith(PropertyNames.EMailAddress);
      return null;
    }
  }
}