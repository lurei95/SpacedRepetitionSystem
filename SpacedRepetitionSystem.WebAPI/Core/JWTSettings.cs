namespace SpacedRepetitionSystem.WebAPI.Core
{
  /// <summary>
  /// Settings for the json web token
  /// </summary>
  public sealed class JWTSettings
  {
    /// <summary>
    /// The secret key
    /// </summary>
    public string SecretKey { get; set; }
  }
}