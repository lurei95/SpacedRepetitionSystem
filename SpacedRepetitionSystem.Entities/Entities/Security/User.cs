using System.Collections.Generic;

namespace SpacedRepetitionSystem.Entities.Entities.Security
{
  /// <summary>
  /// A user of the app
  /// </summary>
  public sealed class User : IEntity
  {
    ///<inheritdoc/>
    public object Id => UserId;

    /// <summary>
    /// UserId
    /// </summary>
    public long UserId { get; set; }

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

    ///<inheritdoc/>
    public string GetDisplayName() => null;

    /// <summary>
    /// Refresh tokens
    /// </summary>
    public List<RefreshToken> RefreshTokens { get; } = new List<RefreshToken>();
  }
}
