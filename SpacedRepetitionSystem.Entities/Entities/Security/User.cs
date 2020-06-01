using SpacedRepetitionSystem.Utility.Extensions;
using System;
using System.Collections.Generic;

namespace SpacedRepetitionSystem.Entities.Entities.Security
{
  /// <summary>
  /// A user of the app
  /// </summary>
  public sealed class User : IRootEntity
  {
    /// <summary>
    /// The test user
    /// </summary>
    public static User GuestUser { get; } = new User()
    {
      UserId = Guid.Parse("58C07E7D-6427-43E2-B318-EBAA9294F8A8"),
      Email = "test@test.com",
      Password = "test"
    };

    ///<inheritdoc/>
    public object Id => UserId;

    ///<inheritdoc/>
    public string Route => "Users";

    /// <summary>
    /// UserId
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Email of the user
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Password
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// The access token
    /// </summary>
    public string AccessToken { get; set; }

    /// <summary>
    /// The refresh token
    /// </summary>
    public string RefreshToken { get; set; }

    /// <summary>
    /// Refresh tokens
    /// </summary>
    public List<RefreshToken> RefreshTokens { get; } = new List<RefreshToken>();

    ///<inheritdoc/>
    public string GetDisplayName()
      => EntityNames.EntityDisplayNameFormat.FormatWith(EntityNameHelper.GetName<User>(), Email);
  }
}