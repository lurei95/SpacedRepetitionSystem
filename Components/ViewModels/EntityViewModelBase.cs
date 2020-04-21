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
    /// The controller used to perform actions on the entity
    /// </summary>
    protected EntityControllerBase<TEntity> Controller { get; private set; }

    /// <summary>
    /// Navigation Manager
    /// </summary>
    protected NavigationManager NavigationManager { get; private set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="context">DbContext (Injected)</param>
    /// <param name="navigationManager">NavigationManager (Injected)</param>
    /// <param name="controller">Controller (Injected)</param>
    public EntityViewModelBase(DbContext context, NavigationManager navigationManager, EntityControllerBase<TEntity> controller)
    {
      NavigationManager = navigationManager;
      Context = context;
      Controller = controller;
    }
  }
}