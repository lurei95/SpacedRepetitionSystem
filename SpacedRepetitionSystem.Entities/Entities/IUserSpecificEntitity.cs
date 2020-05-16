namespace SpacedRepetitionSystem.Entities.Entities
{
  /// <summary>
  /// Interafce for a user specific entity
  /// </summary>
  public interface IUserSpecificEntity : IEntity
  {
    /// <summary>
    /// Id of the user
    /// </summary>
    public long UserId { get; set; }
  }
}