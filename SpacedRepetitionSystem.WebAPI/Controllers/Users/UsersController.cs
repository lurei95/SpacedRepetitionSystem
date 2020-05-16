﻿using SpacedRepetitionSystem.Entities.Entities.Users;
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
using Microsoft.AspNetCore.Authorization;

namespace SpacedRepetitionSystem.Logic.Controllers.Identity
{
  /// <summary>
  /// Controller for <see cref="User"/>
  /// </summary>
  public sealed class UsersController : EntityControllerBase<User>
  {
    private readonly JWTSettings jwtSettings;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="commitValidator">CommitValidator (injected)</param>
    /// <param name="deleteValidator">DeleteValidator (injected)</param>
    /// <param name="context">DBContext (injected)</param>
    public UsersController(DeleteValidatorBase<User> deleteValidator, CommitValidatorBase<User> commitValidator, DbContext context, IOptions<JWTSettings> jwtSettings)
      : base(deleteValidator, commitValidator, context) 
    { this.jwtSettings = jwtSettings.Value;  }

    ///<inheritdoc/>
    [Authorize]
    [HttpGet("{id}")]
    public override async Task<ActionResult<User>> GetAsync(object id)
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
    /// <param name="email">email of the user</param>
    /// <param name="password">password of the user</param>
    /// <returns>User or null</returns>
    [HttpPost("/Login")]
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

    ///<inheritdoc/>
    protected override async Task<IActionResult> PostCoreAsync([FromBody] User entity)
    {
      IActionResult result = await base.PostCoreAsync(entity);
      if (entity != null)
      {
        entity.Password = entity.Password.Encrypt();
        RefreshToken refreshToken = GenerateRefreshToken();
        entity.RefreshTokens.Add(refreshToken);
        entity.RefreshToken = refreshToken.Token;
        entity.AccessToken = GenerateAccessToken(entity.Email);
      }
      return result;
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

    /// <summary>
    /// Gets a user by its accesss token
    /// </summary>
    /// <param name="accessToken">The access token</param>
    /// <returns></returns>
    public async Task<User> GetUserByAccessToken([FromBody] string accessToken)
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
  }
}