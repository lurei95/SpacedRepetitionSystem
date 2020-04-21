using Microsoft.EntityFrameworkCore;
using SpacedRepetitionSystem.Entities.Entities.SmartCards;
using SpacedRepetitionSystem.Logic.Controllers.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Logic.Controllers.SmartCards
{
  /// <summary>
  /// Controller for <see cref="SmartCard"/>
  /// </summary>
  public sealed class SmartCardsController : EntityControllerBase<SmartCard>
  {
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="context">Context used to perform the actions</param>
    public SmartCardsController(DbContext context) : base(context) { }

    ///<inheritdoc/>
    public override void Delete(SmartCard entity)
    {
      UnitOfWork unitOfWork = new UnitOfWork(Context);
      unitOfWork.Execute(() => { Context.Remove(entity); });
    }

    ///<inheritdoc/>
    public override SmartCard Get(object id)
    {
      return Context.Set<SmartCard>()
        .Include(card => card.Fields)
        .Include(card => card.PracticeSet)
        .Include(card => card.SmartCardDefinition)
        .FirstOrDefault(card => card.SmartCardId == (long)id);
    }

    ///<inheritdoc/>
    public override async Task<List<SmartCard>> Get(IDictionary<string, object> searchParameters)
    {
      return await Context.Set<SmartCard>()
        .Include(card => card.Fields)
        .Include(card => card.PracticeSet)
        .Include(card => card.SmartCardDefinition)
        .ToListAsync();
    }

    ///<inheritdoc/>
    public override void Post(SmartCard entity)
    {
      UnitOfWork unitOfWork = new UnitOfWork(Context);
      unitOfWork.Execute(() => Context.Add(entity));
    }

    ///<inheritdoc/>
    public override void Put(SmartCard entity)
    {
      UnitOfWork unitOfWork = new UnitOfWork(Context);
      unitOfWork.Execute(() => { });
    }
  }
}