using SpacedRepetitionSystem.Entities.Validation.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using SpacedRepetitionSystem.Utility.Extensions;
using SpacedRepetitionSystem.WebAPI.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SpacedRepetitionSystem.Entities.Entities.Security;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using Microsoft.AspNetCore.Authorization;
using SpacedRepetitionSystem.Utility.Notification;

namespace SpacedRepetitionSystem.Logic.Controllers.Security
{
  /// <summary>
  /// Controller for <see cref="User"/>
  /// </summary>
  [Route("[controller]")]
  [ApiController]
  public sealed class UsersController : EntityControllerBase<User, Guid>
  {
    private readonly JWTSettings jwtSettings;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="commitValidator">CommitValidator (injected)</param>
    /// <param name="deleteValidator">DeleteValidator (injected)</param>
    /// <param name="context">DBContext (injected)</param>
    /// <param name="jwtSettings">The jwt settings</param>
    public UsersController(DeleteValidatorBase<User> deleteValidator, CommitValidatorBase<User> commitValidator, DbContext context, IOptions<JWTSettings> jwtSettings)
      : base(deleteValidator, commitValidator, context) 
    { this.jwtSettings = jwtSettings.Value;  }

    ///<inheritdoc/>
    [Authorize]
    [HttpGet("{id}")]
    public override async Task<ActionResult<User>> GetAsync([FromRoute] Guid id)
    {
      User user = await Context.Set<User>().FindAsync(id);
      if (user == null)
        return NotFound();
      return user;
    }

    ///<inheritdoc/>
    [HttpGet]
    public override Task<ActionResult<List<User>>> GetAsync(IDictionary<string, object> searchParameters)
    { throw new NotSupportedException(); }

    /// <summary>
    /// Returns user or null if no user exists
    /// </summary>
    /// <param name="user">The user</param>
    /// <returns>User or null</returns>
    [HttpPost("Login")]
    public async Task<ActionResult<User>> Login([FromBody] User user)
    {
      if (user == null)
        return BadRequest();

      string password = user.Password.Encrypt();
      User user1 = await Context.Set<User>()
        .Where(user2 => user2.Email == user.Email && user2.Password == password)
        .FirstOrDefaultAsync();

      if (user1 == null)
        return NotFound();

      RefreshToken refreshToken = GenerateRefreshToken();
      user1.RefreshTokens.Add(refreshToken);
      user1.AccessToken = GenerateAccessToken(user1.UserId);
      user1.RefreshToken = refreshToken.Token;
      await Context.SaveChangesAsync();
      return user1;
    }

    /// <summary>
    /// Returns user or null if no user exists
    /// </summary>
    /// <param name="user">The user</param>
    /// <returns>User or null</returns>
    [HttpPost("Signup")]
    public async Task<ActionResult<User>> Signup([FromBody] User user)
    {
      if (user == null)
        return BadRequest();

      string error = CommitValidator.Validate(user);
      if (!string.IsNullOrEmpty(error))
        throw new NotifyException(error);

      Context.Add(user);
      user.Password = user.Password.Encrypt();
      RefreshToken refreshToken = GenerateRefreshToken();
      user.RefreshTokens.Add(refreshToken);
      user.RefreshToken = refreshToken.Token;
      CreateInitialDataForNewUser(user);
      user.UserId = Guid.NewGuid();
      user.AccessToken = GenerateAccessToken(user.UserId);
      await Context.SaveChangesAsync();

      return user;
    }

    /// <summary>
    /// Refreshes the jwt token for a user
    /// </summary>
    /// <param name="refreshRequest">Request containing the old jwt token and the refresh token</param>
    /// <returns></returns>
    [HttpPost("RefreshToken")]
    public async Task<ActionResult<User>> RefreshToken([FromBody] RefreshRequest refreshRequest)
    {
      if (refreshRequest == null)
        return BadRequest();
      User user = await GetUserFromAccessToken(refreshRequest.AccessToken);
      if (user == null)
        return NotFound();

      if (ValidateRefreshToken(user, refreshRequest.RefreshToken))
      {
        user.AccessToken = GenerateAccessToken(user.UserId);
        return user;
      }
      else
        return Unauthorized();
    }

    /// <summary>
    /// Gets a user by its accesss token
    /// </summary>
    /// <param name="accessToken">The access token</param>
    /// <returns></returns>
    [HttpPost("GetUserByAccessToken")]
    public async Task<User> GetUserByAccessToken([FromBody] string accessToken)
    {
      User user = await GetUserFromAccessToken(accessToken);
      if (user != null)
        return user;
      return null;
    }

    ///<inheritdoc/>
    protected override Task<IActionResult> PostCoreAsync(User entity)
    { throw new NotSupportedException(); }

    ///<inheritdoc/>
    protected override Task<IActionResult> DeleteCoreAsync(User entity)
    { throw new NotSupportedException(); }

    ///<inheritdoc/>
    protected override Task<IActionResult> PutCoreAsync(User entity)
    { throw new NotSupportedException(); }

    private bool ValidateRefreshToken(User user, string refreshToken)
    {
      RefreshToken refreshTokenUser = Context.Set<RefreshToken>()
        .Where(rt => rt.Token == refreshToken)
        .OrderByDescending(rt => rt.ExpirationDate)
        .FirstOrDefault();

      if (refreshTokenUser != null && refreshTokenUser.UserId == user.UserId
        && refreshTokenUser.ExpirationDate > DateTime.UtcNow)
        return true;
      return false;
    }

    private async Task<User> GetUserFromAccessToken(string accessToken)
    {
      try
      {
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        byte[] key = Encoding.ASCII.GetBytes(jwtSettings.SecretKey);

        TokenValidationParameters tokenValidationParameters = new TokenValidationParameters
        {
          ValidateIssuerSigningKey = true,
          IssuerSigningKey = new SymmetricSecurityKey(key),
          ValidateIssuer = false,
          ValidateAudience = false
        };

        ClaimsPrincipal principle = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out SecurityToken securityToken);
        if (securityToken is JwtSecurityToken jwtSecurityToken 
          && jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
          string userId = principle.FindFirst(ClaimTypes.Name)?.Value;
          return await Context.Set<User>()
            .Where(user => user.UserId == Guid.Parse(userId))
            .FirstOrDefaultAsync();
        }
      }
      catch (Exception)
      {
        return new User();
      }
      return new User();
    }

    private RefreshToken GenerateRefreshToken()
    {
      RefreshToken refreshToken = new RefreshToken();

      byte[] randomNumber = new byte[32];
      using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
      {
        rng.GetBytes(randomNumber);
        refreshToken.Token = Convert.ToBase64String(randomNumber);
      }
      refreshToken.ExpirationDate = DateTime.UtcNow.AddMonths(6);

      return refreshToken;
    }

    private string GenerateAccessToken(Guid userId)
    {
      JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
      byte[] key = Encoding.ASCII.GetBytes(jwtSettings.SecretKey);
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

    private void CreateInitialDataForNewUser(User user)
    {
      CardTemplate template = new CardTemplate()
      {
        Title = "Default",
        User = user
      };
      template.FieldDefinitions.Add(new CardFieldDefinition()
      {
        CardTemplate = template,
        FieldName = "Front"
      });
      template.FieldDefinitions.Add(new CardFieldDefinition()
      {
        CardTemplate = template,
        FieldName = "Back"
      });
      Context.Add(template);

      Deck deck = new Deck()
      {
        Title = "Default",
        DefaultCardTemplate = template,
        IsPinned = true,
        User = user
      };
      Context.Add(deck);
    }
  }
}