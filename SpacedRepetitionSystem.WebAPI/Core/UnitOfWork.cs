using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.WebAPI.Core
{
  /// <summary>
  /// Class Representing a unit of work
  /// </summary>
  public sealed class UnitOfWork
  {
    private readonly DbContext context;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="context">The context used to perform the action</param>
    public UnitOfWork(DbContext context)
    { this.context = context; }

    /// <summary>
    /// Executes the action as a unit of work
    /// </summary>
    /// <param name="action">the action to be executed as a unit of work</param>
    public void Execute(Action action)
    {
      try
      {
        action.Invoke();
        context.SaveChanges();
      }
      catch (Exception)
      {
        RerollContext();
        throw;
      }
    }

    /// <summary>
    /// Executes the action async as a unit of work
    /// </summary>
    /// <param name="action">the action to be executed as a unit of work</param>
    /// <returns>Task</returns>
    public async Task ExecuteAsync(Action action)
    {
      try
      {
        action.Invoke();
        await context.SaveChangesAsync();
      }
      catch (Exception ex)
      {
        RerollContext();
        throw;
      }
    }

    private void RerollContext()
    {
      context.ChangeTracker.Entries()
        .Where(e => e.Entity != null).ToList()
        .ForEach(e => e.State = EntityState.Detached);
      context.SaveChanges();
    }
  }
}