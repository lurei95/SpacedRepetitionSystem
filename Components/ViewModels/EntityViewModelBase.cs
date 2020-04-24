using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using SpacedRepetitionSystem.Entities.Entities;
using SpacedRepetitionSystem.Logic.Controllers.Core;

namespace SpacedRepetitionSystem.Components.ViewModels
{
  /// <summary>
  /// Base class for a view model associated with an entity
  /// </summary>
  /// <typeparam name="TEntity"></typeparam>
  public abstract class EntityViewModelBase<TEntity> : ViewModelBase where  TEntity : IEntity
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
    /// Navigation Manager
    /// </summary>
    protected NavigationManager NavigationManager { get; private set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="context">DbContext (Injected)</param>
    /// <param name="navigationManager">NavigationManager (Injected)</param>
    /// <param name="apiConnector">ApiConnetcor (Injected)</param>
    public EntityViewModelBase(DbContext context, NavigationManager navigationManager, IApiConnector apiConnector)
    {
      NavigationManager = navigationManager;
      Context = context;
      ApiConnector = apiConnector;
    }
  }
}