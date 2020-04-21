using Microsoft.EntityFrameworkCore;
using SpacedRepetitionSystem.Entities.Entities.SmartCards;
using SpacedRepetitionSystem.Logic.Controllers.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Logic.Controllers.SmartCards
{
  /// <summary>
  /// Controller for <see cref="SmartCardDefinition"/>
  /// </summary>
  public sealed class SmartCardDefinitionsController : EntityControllerBase<SmartCardDefinition>
  {
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="context">Context used to perform the actions</param>
    public SmartCardDefinitionsController(DbContext context) : base(context) { }

    ///<inheritdoc/>
    public override void Delete(SmartCardDefinition entity)
    {
      UnitOfWork unitOfWork = new UnitOfWork(Context);
      unitOfWork.Execute(() => { Context.Remove(entity); });
    }

    ///<inheritdoc/>
    public override SmartCardDefinition Get(object id)
    {
      return Context.Set<SmartCardDefinition>()
        .Include(definition => definition.FieldDefinitions)
        .FirstOrDefault(card => card.SmartCardDefinitionId == (long)id);
    }

    ///<inheritdoc/>
    public override async Task<List<SmartCardDefinition>> Get(IDictionary<string, object> searchParameters)
    {
      return await Context.Set<SmartCardDefinition>()
        .Include(definition => definition.FieldDefinitions)
        .ToListAsync();
    }

    ///<inheritdoc/>
    public override void Post(SmartCardDefinition entity)
    {
      UnitOfWork unitOfWork = new UnitOfWork(Context);
      unitOfWork.Execute(() => Context.Add(entity));
    }

    ///<inheritdoc/>
    public override void Put(SmartCardDefinition entity)
    {
      UnitOfWork unitOfWork = new UnitOfWork(Context);
      unitOfWork.Execute(() => { });
    }
  }
}