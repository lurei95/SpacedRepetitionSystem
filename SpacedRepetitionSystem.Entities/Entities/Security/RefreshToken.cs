﻿using System;

namespace SpacedRepetitionSystem.Entities.Entities.Security
{
  /// <summary>
  /// Refresh token for a user
  /// </summary>
  public sealed class RefreshToken : IUserSpecificEntity
  {
    ///<inheritdoc/>
    public object Id => TokenId;

    /// <summary>
    /// Id of the token
    /// </summary>
    public long TokenId { get; set; }

    /// <summary>
    /// The id of the user
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// The token
    /// </summary>
    public string Token { get; set; }

    /// <summary>
    /// Expiration date for the token
    /// </summary>
    public DateTime ExpirationDate { get; set; }

    /// <summary>
    /// The user
    /// </summary>
    public User User { get; set; }

    ///<inheritdoc/>
    public string GetDisplayName() => null;
  }
}