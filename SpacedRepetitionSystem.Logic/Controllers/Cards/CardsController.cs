using Microsoft.EntityFrameworkCore;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Logic.Controllers.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Logic.Controllers.Cards
{
  /// <summary>
  /// Controller for <see cref="Card"/>
  /// </summary>
  public sealed class CardsController : EntityControllerBase<Card>
  {
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="context">Context used to perform the actions</param>
    public CardsController(DbContext context) : base(context) { }

    ///<inheritdoc/>
    public override void Delete(Card entity)
    {
      UnitOfWork unitOfWork = new UnitOfWork(Context);
      unitOfWork.Execute(() => { Context.Remove(entity); });
    }

    ///<inheritdoc/>
    public override Card Get(object id)
    {
      return Context.Set<Card>()
        .Include(card => card.Fields)
        .Include(card => card.Deck)
        .Include(card => card.CardTemplate)
        .FirstOrDefault(card => card.CardId == (long)id);
    }

    ///<inheritdoc/>
    public override async Task<List<Card>> Get(IDictionary<string, object> searchParameters)
    {
      return await Context.Set<Card>()
        .Include(card => card.Fields)
        .Include(card => card.Deck)
        .Include(card => card.CardTemplate)
        .ToListAsync();
    }

    ///<inheritdoc/>
    public override void Post(Card entity)
    {
      UnitOfWork unitOfWork = new UnitOfWork(Context);
      unitOfWork.Execute(() => Context.Add(entity));
    }

    ///<inheritdoc/>
    public override void Put(Card entity)
    {
      UnitOfWork unitOfWork = new UnitOfWork(Context);
      unitOfWork.Execute(() => { });
    }
  }
}