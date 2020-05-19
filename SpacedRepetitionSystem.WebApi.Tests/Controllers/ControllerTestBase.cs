using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SpacedRepetitionSystem.Entities.Entities.Security;
using SpacedRepetitionSystem.Utility.Extensions;
using System;
using System.Security.Claims;

namespace SpacedRepetitionSystem.WebApi.Tests.Controllers
{
  /// <summary>
  /// BaseClass for unit tests targeting controllers
  /// </summary>
  public abstract class ControllerTestBase : EntityFrameWorkTestBase
  {
    /// <summary>
    /// The user
    /// </summary>
    protected static User User { get; } = new User()
    {
      UserId = Guid.NewGuid(),
      Email = "test@test.com",
      Password = "test".Encrypt()
    };

    /// <summary>
    /// Controller context
    /// </summary>
    protected ControllerContext ControllerContext { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    public ControllerTestBase()
    {
      var identity = new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, Convert.ToString(User.UserId)) }, "TestAuthType");
      var claimsPrincipal = new ClaimsPrincipal(identity);
      ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = claimsPrincipal } };
    }
  }
}