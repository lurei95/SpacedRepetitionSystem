using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using SpacedRepetitionSystem.Entities.Entities.Security;
using SpacedRepetitionSystem.Entities.Validation.Core;

namespace SpacedRepetitionSystem.Entities.Validation.Cards
{
  /// <summary>
  /// CommitValidator for <see cref="User"/>
  /// </summary>
  public sealed class UserCommitValidator : CommitValidatorBase<User>
  {
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="context">Context (Injected)</param>
    public UserCommitValidator(DbContext context) : base(context) { }

    ///<inheritdoc/>
    public override string Validate(User entity)
    {
      if (Context.Set<User>().AsNoTracking().Any(user => user.Email == entity.Email))
        return Errors.UserAlreadyExists;
      return null;
    }
  }
}