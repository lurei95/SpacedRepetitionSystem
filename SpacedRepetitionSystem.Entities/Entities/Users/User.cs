namespace SpacedRepetitionSystem.Entities.Entities.Users
{
  /// <summary>
  /// A user of the app
  /// </summary>
  public sealed class User : IEntity
  {
    ///<inheritdoc/>
    public object Id => UserId;

    /// <summary>
    /// Id of the user
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// E-mail address
    /// </summary>
    public string EmailAddress { get; set; }

    /// <summary>
    /// Password
    /// </summary>
    public string Password { get; set; }

    public string ConfirmPassword { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }

    ///<inheritdoc/>
    public string GetDisplayName() => null;
  }
}
