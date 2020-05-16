using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpacedRepetitionSystem.Entities.Entities;
using SpacedRepetitionSystem.Entities.Validation.Core;
using SpacedRepetitionSystem.Utility.Notification;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.WebAPI.Core
{
  /// <summary>
  /// Base class for a controller for a specific entity
  /// </summary>
  /// <typeparam name="TEntity"></typeparam>
  public abstract class EntityControllerBase<TEntity> : ControllerBase where TEntity : class, IEntity
  {
    private readonly DeleteValidatorBase<TEntity> deleteValidator;
    private readonly CommitValidatorBase<TEntity> commitValidator;

    /// <summary>
    /// Context used to perform the actions
    /// </summary>
    public DbContext Context { get; private set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="commitValidator">CommitValidator (injected)</param>
    /// <param name="deleteValidator">DeleteValidator (injected)</param>
    public EntityControllerBase(DeleteValidatorBase<TEntity> deleteValidator, CommitValidatorBase<TEntity> commitValidator, DbContext context) 
    { 
      this.deleteValidator = deleteValidator;
      Context = context;
      this.commitValidator = commitValidator;
    }

    /// <summary>
    /// Returns the entity with the Id
    /// </summary>
    /// <param name="id">Id of the entity</param>
    /// <returns>The entity with the Id</returns>
    public abstract Task<ActionResult<TEntity>> GetAsync(object id);

    /// <summary>
    /// Returns a list of entities matching the filter
    /// </summary>
    /// <param name="searchParameters">A dictionary containing the search parameters</param>
    /// <returns>A list of entities matching the filter</returns>
    public abstract Task<ActionResult<List<TEntity>>> GetAsync(IDictionary<string, object> searchParameters);

    /// <summary>
    /// Updates an existing entity
    /// </summary>
    /// <param name="entity">The updated entity</param>
    [HttpPut]
    public async Task<IActionResult> PutAsync(TEntity entity)
    {
      string error = commitValidator.Validate(entity);
      if (string.IsNullOrEmpty(error))
      {
        UnitOfWork unitOfWork = new UnitOfWork(Context);
        IActionResult result = null;
        await unitOfWork.ExecuteAsync(async () => result = await PutCoreAsync(entity));
        return result;
      }
      else
        throw new NotifyException(error);
    }

    /// <summary>
    /// Creates a new entity
    /// </summary>
    /// <param name="entity">The new entity</param>
    [HttpPost]
    public async Task<IActionResult> PostAsync(TEntity entity)
    {
      string error = commitValidator.Validate(entity);
      if (string.IsNullOrEmpty(error))
      {
        UnitOfWork unitOfWork = new UnitOfWork(Context);
        IActionResult result = null;
        await unitOfWork.ExecuteAsync(async () => result = await PostCoreAsync(entity));
        return result;
      }
      else
        throw new NotifyException(error);
    }

    /// <summary>
    /// Deletes an existing entity
    /// </summary>
    /// <param name="entity">The entity to delete</param>
    [HttpDelete]
    public async Task<IActionResult> DeleteAsync(TEntity entity)
    {
      string error = deleteValidator.Validate(entity);
      if (string.IsNullOrEmpty(error))
      {
        UnitOfWork unitOfWork = new UnitOfWork(Context);
        IActionResult result = null;
        await unitOfWork.ExecuteAsync(async () => result = await DeleteCoreAsync(entity));
        return result;
      }
      else
        throw new NotifyException(error);
    }

    /// <summary>
    /// Updates an existing entity
    /// </summary>
    /// <param name="entity">The updated entity</param>
    protected virtual async Task<IActionResult> PutCoreAsync(TEntity entity) 
    {
      if (entity == null)
        return await Task.FromResult(BadRequest());
      Context.Entry(entity).State = EntityState.Modified;
      return await Task.FromResult(NoContent());
    }

    /// <summary>
    /// Deletes an existing entity
    /// </summary>
    /// <param name="entity">The entity to delete</param>
    protected virtual async Task<IActionResult> DeleteCoreAsync(TEntity entity)
    {
      if (entity == null)
        return BadRequest();
      TEntity entity1 = await Context.FindAsync<TEntity>(entity.Id);
      if (entity1 == null)
        return NotFound();
      Context.Remove(entity1);
      return NoContent();
    }

    /// <summary>
    /// Creates a new entity
    /// </summary>
    /// <param name="entity">The new entity</param>
    protected virtual async Task<IActionResult> PostCoreAsync(TEntity entity)
    {
      if (entity == null)
        return await Task.FromResult(BadRequest());
      if (entity is IUserSpecificEntity userSpecificEntity)
        userSpecificEntity.UserId = GetUserId();
      Context.Add(entity);
      return await Task.FromResult(NoContent());
    }

    /// <summary>
    /// Returns the id of the user authenticated by the jwt
    /// </summary>
    /// <returns></returns>
    protected long GetUserId() => long.Parse(User.Identity.Name);
  }
}