using Microsoft.EntityFrameworkCore;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Logic.Controllers.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Logic.Controllers.Cards
{
  /// <summary>
  /// Controller for <see cref="CardTemplate"/>
  /// </summary>
  public sealed class CardTemplatesController : EntityControllerBase<CardTemplate>
  {
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="context">Context used to perform the actions</param>
    public CardTemplatesController(DbContext context) : base(context) { }

    ///<inheritdoc/>
    public override void Delete(CardTemplate entity)
    {
      UnitOfWork unitOfWork = new UnitOfWork(Context);
      unitOfWork.Execute(() => { Context.Remove(entity); });
    }

    ///<inheritdoc/>
    public override CardTemplate Get(object id)
    {
      return Context.Set<CardTemplate>()
        .Include(definition => definition.FieldDefinitions)
        .FirstOrDefault(card => card.CardTemplateId == (long)id);
    }

    ///<inheritdoc/>
    public override async Task<List<CardTemplate>> Get(IDictionary<string, object> searchParameters)
    {
      return await Context.Set<CardTemplate>()
        .Include(definition => definition.FieldDefinitions)
        .ToListAsync();
    }

    ///<inheritdoc/>
    public override void Post(CardTemplate entity)
    {
      UnitOfWork unitOfWork = new UnitOfWork(Context);
      unitOfWork.Execute(() => Context.Add(entity));
    }

    ///<inheritdoc/>
    public override void Put(CardTemplate entity)
    {
      UnitOfWork unitOfWork = new UnitOfWork(Context);
      unitOfWork.Execute(() => { });
    }
  }
}