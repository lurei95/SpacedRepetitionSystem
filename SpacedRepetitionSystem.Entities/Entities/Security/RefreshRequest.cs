namespace SpacedRepetitionSystem.Entities.Entities.Security
{
  /// <summary>
  /// Request for refreshing the jwt of a user
  /// </summary>
  public sealed class RefreshRequest
  {
    /// <summary>
    /// The access token
    /// </summary>
    public string AccessToken { get; set; }

    /// <summary>
    /// The refresh token
    /// </summary>
    public string RefreshToken { get; set; }
  }
}