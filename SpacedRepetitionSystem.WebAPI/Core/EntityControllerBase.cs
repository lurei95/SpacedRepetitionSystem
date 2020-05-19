using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Internal;
using SpacedRepetitionSystem.Entities.Entities;
using SpacedRepetitionSystem.Entities.Validation.Core;
using SpacedRepetitionSystem.Utility.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.WebAPI.Core
{
  /// <summary>
  /// Base class for a controller for a specific entity
  /// </summary>
  /// <typeparam name="TEntity"></typeparam>
  /// <typeparam name="TKey">Type of the primary key</typeparam>
  public abstract class EntityControllerBase<TEntity, TKey> : ControllerBase where TEntity : class, IEntity
  {
    /// <summary>
    /// DeleteValdiator
    /// </summary>
    protected DeleteValidatorBase<TEntity> DeleteValidator { get; private set; }

    /// <summary>
    /// CommitValidator
    /// </summary>
    protected CommitValidatorBase<TEntity> CommitValidator { get; private set; }

    /// <summary>
    /// Context used to perform the actions
    /// </summary>
    public DbContext Context { get; private set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="commitValidator">CommitValidator (injected)</param>
    /// <param name="deleteValidator">DeleteValidator (injected)</param>
    /// <param name="context">DBContext (injected)</param>
    public EntityControllerBase(DeleteValidatorBase<TEntity> deleteValidator, CommitValidatorBase<TEntity> commitValidator, DbContext context) 
    { 
      DeleteValidator = deleteValidator;
      CommitValidator = commitValidator;
      Context = context;
    }

    /// <summary>
    /// Returns the entity with the Id
    /// </summary>
    /// <param name="id">Id of the entity</param>
    /// <returns>The entity with the Id</returns>
    public abstract Task<ActionResult<TEntity>> GetAsync(TKey id);

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
    public async Task<IActionResult> PutAsync([FromBody] TEntity entity)
    {
      if (entity == null)
        return await Task.FromResult(BadRequest());
      string error = CommitValidator.Validate(entity);
      if (string.IsNullOrEmpty(error))
      {
        IActionResult result = await PutCoreAsync(entity);
        await Context.SaveChangesAsync();
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
    public async Task<IActionResult> PostAsync([FromBody] TEntity entity)
    {
      if (entity == null)
        return await Task.FromResult(BadRequest());
      string error = CommitValidator.Validate(entity);
      if (string.IsNullOrEmpty(error))
      {
        IActionResult result = await PostCoreAsync(entity);
        await Context.SaveChangesAsync();
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
    public async Task<IActionResult> DeleteAsync([FromBody] TEntity entity)
    {
      if (entity == null)
        return BadRequest();
      string error = DeleteValidator.Validate(entity);
      if (string.IsNullOrEmpty(error))
      {
        IActionResult result = await DeleteCoreAsync(entity);
        await Context.SaveChangesAsync();
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
      TEntity exisiting = await Context.FindAsync<TEntity>(entity.Id);
      if (exisiting == null)
        return NotFound();
      Context.Entry(exisiting).State = EntityState.Detached;
      Context.Entry(entity).State = EntityState.Modified;
      return await Task.FromResult(Ok());
    }

    /// <summary>
    /// Deletes an existing entity
    /// </summary>
    /// <param name="entity">The entity to delete</param>
    protected virtual async Task<IActionResult> DeleteCoreAsync(TEntity entity)
    {
      TEntity entity1 = await Context.FindAsync<TEntity>(entity.Id);
      if (entity1 == null)
        return NotFound();
      Context.Remove(entity1);
      return Ok();
    }

    /// <summary>
    /// Creates a new entity
    /// </summary>
    /// <param name="entity">The new entity</param>
    protected virtual async Task<IActionResult> PostCoreAsync(TEntity entity)
    {
      if (entity is IUserSpecificEntity userSpecificEntity)
        userSpecificEntity.UserId = GetUserId();
      Context.Add(entity);
      return await Task.FromResult(Ok());
    }

    /// <summary>
    /// Returns the id of the user authenticated by the jwt
    /// </summary>
    /// <returns></returns>
    protected Guid GetUserId() => Guid.Parse(User.Identity.Name);
  }
}