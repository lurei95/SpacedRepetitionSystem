using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpacedRepetitionSystem.Entities.Entities.Security;
using SpacedRepetitionSystem.Entities.Validation.Cards;
using SpacedRepetitionSystem.Logic.Controllers.Security;
using SpacedRepetitionSystem.Utility.Extensions;
using SpacedRepetitionSystem.Utility.Notification;
using SpacedRepetitionSystem.WebAPI.Core;
using SpacedRepetitionSystem.WebAPI.Validation.Core;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.WebApi.Tests.Controllers.Cards
{
  /// <summary>
  /// Testclass for <see cref="UsersController"/>
  /// </summary>
  [TestClass]
  public sealed class UsersControllerTests : ControllerTestBase
  {
    private static RefreshToken invalidRefreshToken;
    private static RefreshToken validRefreshToken;
    private static readonly string secretKey = "thisisasecretkeyanddontsharewithanyone";

    /// <summary>
    /// Creates the test data when the class is initialized
    /// </summary>
    /// <param name="context">TestContext</param>
    [ClassInitialize]
#pragma warning disable IDE0060 // Remove unused parameter
    public static void ClassInitialize(TestContext context)
#pragma warning restore IDE0060 // Remove unused parameter
    {
      validRefreshToken = GenerateRefreshToken(DateTime.Today.AddDays(1));
      invalidRefreshToken = GenerateRefreshToken(DateTime.Today.AddDays(-1));
      User.RefreshTokens.Add(validRefreshToken);
      User.RefreshTokens.Add(invalidRefreshToken);
    }

    ///<inheritdoc/>
    [TestInitialize]
    public override void TestInitialize()
    {
      base.TestInitialize();
      CreateData(context => { context.Add(User); });
    }

    /// <summary>
    /// Tests <see cref="UsersController.GetAsync(Guid)"/>
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task GetUserByIdTest()
    {
      using DbContext context = CreateContext();
      UsersController controller = CreateController(context);

      //get user successfully
      ActionResult<User> result = await controller.GetAsync(User.UserId);
      Assert.IsNotNull(result.Value);
      Assert.AreEqual(User.Email, result.Value.Email);

      //User does not exist -> not found
      result = await controller.GetAsync(Guid.NewGuid());
      Assert.IsTrue(result.Result is NotFoundResult);
    }

    /// <summary>
    /// Tests <see cref="UsersController.GetAsync(IDictionary{string, object})/>
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task GetUsersNotSupportedTest()
    {
      using DbContext context = CreateContext();
      UsersController controller = CreateController(context);
      Dictionary<string, object> parameters = new Dictionary<string, object>();

      bool wasThrown = false;
      try
      {
        await controller.GetAsync(parameters);
      }
      catch (NotSupportedException) { wasThrown = true; }
      Assert.IsTrue(wasThrown);
    }

    /// <summary>
    /// Tests <see cref="UsersController.PostAsync(User entity)/>
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task PostUserNotSupportedTest()
    {
      using DbContext context = CreateContext();
      UsersController controller = CreateController(context);

      //null as parameter -> bad request
      IActionResult result = await controller.PostAsync(null);
      Assert.IsTrue(result is BadRequestResult);

      //Create new valid entity
      bool wasThrown = false;
      try
      {
        await controller.PostAsync(new User());
      }
      catch (NotSupportedException) { wasThrown = true; }
      Assert.IsTrue(wasThrown);
    }

    /// <summary>
    /// Tests <see cref="UsersController.DeleteAsync(User entity)/>
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task DeleteUserNotSupportedTest()
    {
      using DbContext context = CreateContext();
      UsersController controller = CreateController(context);

      //null as parameter -> bad request
      IActionResult result = await controller.PostAsync(null);
      Assert.IsTrue(result is BadRequestResult);

      //Create new valid entity
      bool wasThrown = false;
      try
      {
        await controller.DeleteAsync(new User());
      }
      catch (NotSupportedException) { wasThrown = true; }
      Assert.IsTrue(wasThrown);
    }

    /// <summary>
    /// Tests <see cref="UsersController.PutAsync(user entity)/>
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task PutUserNotSupportedTest()
    {
      using DbContext context = CreateContext();
      UsersController controller = CreateController(context);

      //null as parameter -> bad request
      IActionResult result = await controller.PostAsync(null);
      Assert.IsTrue(result is BadRequestResult);

      //Create new valid entity
      bool wasThrown = false;
      try
      {
        await controller.PutAsync(new User());
      }
      catch (NotSupportedException)
      {
        wasThrown = true;
      }
      Assert.IsTrue(wasThrown);
      Assert.IsTrue(wasThrown);
    }

    /// <summary>
    /// Tests <see cref="UsersController.Login(User)"/>
    /// </summary>
    [TestMethod]
    public async Task LoginTest()
    {
      using DbContext context = CreateContext();
      UsersController controller = CreateController(context);

      //null as parameter -> bad request
      ActionResult<User> result = await controller.Login(null);
      Assert.IsTrue(result.Result is BadRequestResult);

      //User does not exist parameter -> not found
      result = await controller.Login(new User { Email = "test", Password = "test" });
      Assert.IsTrue(result.Result is NotFoundResult);

      //Existing user
      result = await controller.Login(new User { Email = "test@test.com", Password = "test" });
      Assert.IsNotNull(result.Value);
      Assert.IsFalse(string.IsNullOrEmpty(result.Value.RefreshToken));
      Assert.IsFalse(string.IsNullOrEmpty(result.Value.AccessToken));
      bool refreshTokenWasCreated = context.Set<RefreshToken>()
        .Any(token => token.UserId == User.UserId && token.Token == result.Value.RefreshToken);
      Assert.IsTrue(refreshTokenWasCreated);
    }

    /// <summary>
    /// Tests <see cref="UsersController.Signup(User)"/>
    /// </summary>
    [TestMethod]
    public async Task SignupTest()
    {
      using DbContext context = CreateContext();
      UsersController controller = CreateController(context);

      //null as parameter -> bad request
      ActionResult<User> result = await controller.Signup(null);
      Assert.IsTrue(result.Result is BadRequestResult);

      //User with same email already exists -> error
      bool wasThrown = false;
      try
      {
        result = await controller.Signup(new User { Email = "test@test.com", Password = "test" });
      }
      catch (NotifyException) { wasThrown = true; }     
      Assert.IsTrue(wasThrown);

      //Create new user successfully
      result = await controller.Signup(new User { Email = "test1@test1.com", Password = "test1" });
      Assert.IsNotNull(result.Value);
      Assert.IsFalse(string.IsNullOrEmpty(result.Value.RefreshToken));
      Assert.IsFalse(string.IsNullOrEmpty(result.Value.AccessToken));
      RefreshToken refreshToken = await context.Set<RefreshToken>()
        .FirstOrDefaultAsync(token => token.UserId == result.Value.UserId);
      Assert.IsNotNull(refreshToken);
      Assert.AreEqual(result.Value.RefreshToken, refreshToken.Token);
    }

    /// <summary>
    /// Tests <see cref="UsersController.RefreshToken(RefreshRequest)"/>
    /// </summary>
    [TestMethod]
    public async Task RefreshTokenTest()
    {
      using DbContext context = CreateContext();
      UsersController controller = CreateController(context);

      //null as parameter -> BadRequest
      ActionResult<User> result = await controller.RefreshToken(null);
      Assert.IsTrue(result.Result is BadRequestResult);

      //User does not exist -> NotFound
      string otherUserToken = GenerateAccessToken(Guid.NewGuid());
      result = await controller.RefreshToken(new RefreshRequest() { AccessToken = otherUserToken });
      Assert.IsTrue(result.Result is NotFoundResult);

      //Refresh token invalid -> Unauthorized
      string token = GenerateAccessToken(User.UserId);
      result = await controller.RefreshToken(new RefreshRequest() { AccessToken = token,  RefreshToken = invalidRefreshToken.Token });
      Assert.IsTrue(result.Result is UnauthorizedResult);

      //Successfull test
      result = await controller.RefreshToken(new RefreshRequest() { AccessToken = token, RefreshToken = validRefreshToken.Token });
      Assert.IsNotNull(result.Value);
      Assert.IsNotNull(result.Value.AccessToken);
    }

    /// <summary>
    /// Tests <see cref="UsersController.GetUserByAccessToken(string)"/>
    /// </summary>
    [TestMethod]
    public async Task GetUserByAccessTokenTest()
    {
      using DbContext context = CreateContext();
      UsersController controller = CreateController(context);

      //null as parameter -> BadRequest
      ActionResult<User> result = await controller.GetUserByAccessToken(null);
      Assert.IsTrue(result.Result is BadRequestResult);

      //User does not exist -> NotFound
      string otherUserToken = GenerateAccessToken(Guid.NewGuid());
      result = await controller.GetUserByAccessToken(otherUserToken);
      Assert.IsTrue(result.Result is NotFoundResult);

      //User exists -> returns correct user
      string token = GenerateAccessToken(User.UserId);
      result = await controller.GetUserByAccessToken(token);
      Assert.IsNotNull(result.Value);
      Assert.AreEqual(User.UserId, result.Value.UserId);
    }

    private string GenerateAccessToken(Guid userId)
    {
      JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
      byte[] key = Encoding.ASCII.GetBytes(secretKey);
      SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
      {
        Subject = new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, Convert.ToString(userId)) }),
        Expires = DateTime.UtcNow.AddDays(1),
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
          SecurityAlgorithms.HmacSha256Signature)
      };
      SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
      return tokenHandler.WriteToken(token);
    }

    private static RefreshToken GenerateRefreshToken(DateTime expirationDate)
    {
      RefreshToken refreshToken = new RefreshToken();

      byte[] randomNumber = new byte[32];
      using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
      {
        rng.GetBytes(randomNumber);
        refreshToken.Token = Convert.ToBase64String(randomNumber);
      }
      refreshToken.ExpirationDate = expirationDate;
      return refreshToken;
    }

    private UsersController CreateController(DbContext context)
    {
      JWTSettings settings = new JWTSettings() { SecretKey = "thisisasecretkeyanddontsharewithanyone" };
      IOptions<JWTSettings> jwtSettings = Options.Create(settings);
      return new UsersController(new DeleteValidatorBase<User>(context),
        new UserCommitValidator(context), context, jwtSettings)
      { ControllerContext = ControllerContext };
    }
  }
}