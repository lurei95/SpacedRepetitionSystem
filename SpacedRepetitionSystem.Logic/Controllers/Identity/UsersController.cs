using SpacedRepetitionSystem.Entities.Entities.Users;
using SpacedRepetitionSystem.Entities.Validation.Core;
using SpacedRepetitionSystem.Logic.Controllers.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Logic.Controllers.Identity
{
  /// <summary>
  /// Controller for <see cref="User"/>
  /// </summary>
  public sealed class UsersController : EntityControllerBase<User>
  {
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="commitValidator">CommitValidator (injected)</param>
    /// <param name="deleteValidator">DeleteValidator (injected)</param>
    public UsersController(DeleteValidatorBase<User> deleteValidator,
      CommitValidatorBase<User> commitValidator)
      : base(deleteValidator, commitValidator) { }

    ///<inheritdoc/>
    public override User Get(object id) => Context.Set<User>().Find(id);

    ///<inheritdoc/>
    public override Task<List<User>> Get(IDictionary<string, object> searchParameters)
    { throw new NotSupportedException(); }
   
    /// <summary>
    /// Returns user or null if no user exists
    /// </summary>
    /// <param name="email">email of the user</param>
    /// <param name="password">password of the user</param>
    /// <returns>User or null</returns>
    public User Login(string email, string password)
    {
      return Context.Set<User>()
        .Where(user => user.EmailAddress == email && user.Password == password)
        .SingleOrDefault();
    }
  }
}
