using SpacedRepetitionSystem.Entities.Entities.Users;
using SpacedRepetitionSystem.Entities.Validation.Core;
using SpacedRepetitionSystem.Logic.Controllers.Core;
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

namespace SpacedRepetitionSystem.Logic.Controllers.Identity
{
  /// <summary>
  /// Controller for <see cref="User"/>
  /// </summary>
  public sealed class UsersController : EntityControllerBase<User>
  {
    private static readonly string secretKey = "thisisasecretkeyanddontsharewithanyone";

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="commitValidator">CommitValidator (injected)</param>
    /// <param name="deleteValidator">DeleteValidator (injected)</param>
    public UsersController(DeleteValidatorBase<User> deleteValidator,
      CommitValidatorBase<User> commitValidator)
      : base(deleteValidator, commitValidator) 
    { }

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
    public async Task<User> Login(string email, string password)
    {
      password = password.Encrypt();
      User user = await Context.Set<User>()
        .Where(user => user.Email == email && user.Password == password)
        .FirstOrDefaultAsync();

      if(user != null)
      {
        RefreshToken refreshToken = GenerateRefreshToken();
        user.RefreshTokens.Add(refreshToken);
        await Context.SaveChangesAsync();
        user.AccessToken = GenerateAccessToken(user.Email);
        user.RefreshToken = refreshToken.Token;
      }     
      return user;
    }

    protected override void PostCore(User entity)
    {
      base.PostCore(entity);
      if (entity != null)
      {
        entity.Password = entity.Password.Encrypt();
        RefreshToken refreshToken = GenerateRefreshToken();
        entity.RefreshTokens.Add(refreshToken);
        entity.RefreshToken = refreshToken.Token;
        entity.AccessToken = GenerateAccessToken(entity.Email);
      }
    }

    //public async Task<User> RefreshToken([FromBody] RefreshRequest refreshRequest)
    //{
    //  User user = await GetUserFromAccessToken(refreshRequest.AccessToken);

    //  if (user != null && ValidateRefreshToken(user, refreshRequest.RefreshToken))
    //  {
    //    UserWithToken userWithToken = new UserWithToken(user);
    //    userWithToken.AccessToken = GenerateAccessToken(user.UserId);

    //    return userWithToken;
    //  }

    //  return null;
    //}


    public async Task<User> GetUserByAccessToken(string accessToken)
    {
      User user = await GetUserFromAccessToken(accessToken);
      if (user != null)
        return user;
      return null;
    }

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
        byte[] key = Encoding.ASCII.GetBytes(secretKey);

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
            .Where(u => u.Email == userId).FirstOrDefaultAsync();
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

    private string GenerateAccessToken(string userId)
    {
      JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
      byte[] key = Encoding.ASCII.GetBytes(secretKey);
      SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
      {
        Subject = new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, userId) }),
        Expires = DateTime.UtcNow.AddDays(1),
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
          SecurityAlgorithms.HmacSha256Signature)
      };
      SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
      return tokenHandler.WriteToken(token);
    }
  }
}