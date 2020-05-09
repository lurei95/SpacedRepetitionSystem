using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using SpacedRepetitionSystem.Entities.Entities;
using SpacedRepetitionSystem.Logic.Controllers.Core;

namespace SpacedRepetitionSystem.Components.ViewModels
{
  /// <summary>
  /// Base class for a view model associated with an entity
  /// </summary>
  /// <typeparam name="TEntity">Entity-Type</typeparam>
  public abstract class EntityViewModelBase<TEntity> : PageViewModelBase where TEntity : IEntity
  {
    /// <summary>
    /// Context
    /// </summary>
    public DbContext Context { get; private set; }

    /// <summary>
    /// API-Connector to perform operations on entitites
    /// </summary>
    protected IApiConnector ApiConnector { get; private set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="navigationManager">NavigationManager (Injected)</param>
    /// <param name="apiConnector">ApiConnetcor (Injected)</param>
    public EntityViewModelBase(NavigationManager navigationManager, IApiConnector apiConnector)
      : base(navigationManager)
    {
      Context = apiConnector.Context;
      ApiConnector = apiConnector;
    }
  }
}