using Microsoft.EntityFrameworkCore;
using SpacedRepetitionSystem.Entities.Entities.SmartCards;
using SpacedRepetitionSystem.Logic.Controllers.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Logic.Controllers.SmartCards
{
  /// <summary>
  /// Controller for <see cref="PracticeSet"/>
  /// </summary>
  public sealed class PracticeSetsController : EntityControllerBase<PracticeSet>
  {
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="context">Context used to perform the actions</param>
    public PracticeSetsController(DbContext context) : base(context) { }

    ///<inheritdoc/>
    public override void Delete(PracticeSet entity)
    {
      UnitOfWork unitOfWork = new UnitOfWork(Context);
      unitOfWork.Execute(() => { Context.Remove(entity); });
    }

    ///<inheritdoc/>
    public override PracticeSet Get(object id)
    {
      return Context.Set<PracticeSet>()
        .FirstOrDefault(card => card.PracticeSetId == (long)id);
    }

    ///<inheritdoc/>
    public override async Task<List<PracticeSet>> Get(IDictionary<string, object> searchParameters)
    {
      return await Context.Set<PracticeSet>()
        .ToListAsync();
    }

    ///<inheritdoc/>
    public override void Post(PracticeSet entity)
    {
      UnitOfWork unitOfWork = new UnitOfWork(Context);
      unitOfWork.Execute(() => Context.Add(entity));
    }

    ///<inheritdoc/>
    public override void Put(PracticeSet entity)
    {
      UnitOfWork unitOfWork = new UnitOfWork(Context);
      unitOfWork.Execute(() => { });
    }
  }
}