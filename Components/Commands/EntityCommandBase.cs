using SpacedRepetitionSystem.Components.Middleware;
using SpacedRepetitionSystem.Entities.Entities;

namespace SpacedRepetitionSystem.Components.Commands
{
  /// <summary>
  /// Base class for command for a certain entity
  /// </summary>
  /// <typeparam name="TEntity">Type of the entity</typeparam>
  public abstract class EntityCommandBase<TEntity> : CommandBase where TEntity : IEntity
  {
    /// <summary>
    /// The entity
    /// </summary>
    public TEntity Entity { get; set; }

    /// <summary>
    /// ApiConnector
    /// </summary>
    protected IApiConnector ApiConnector { get; private set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="apiConnector">ApiConnector</param>
    public EntityCommandBase(IApiConnector apiConnector)
    { ApiConnector = apiConnector; }
  }
}