using Microsoft.EntityFrameworkCore;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Logic.Controllers.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Logic.Controllers.Cards
{
  /// <summary>
  /// Controller for <see cref="Deck"/>
  /// </summary>
  public sealed class DecksController : EntityControllerBase<Deck>
  {
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="context">Context used to perform the actions</param>
    public DecksController(DbContext context) : base(context) { }

    ///<inheritdoc/>
    public override void Delete(Deck entity)
    {
      UnitOfWork unitOfWork = new UnitOfWork(Context);
      unitOfWork.Execute(() => { Context.Remove(entity); });
    }

    ///<inheritdoc/>
    public override Deck Get(object id)
    {
      return Context.Set<Deck>()
        .FirstOrDefault(card => card.DeckId == (long)id);
    }

    ///<inheritdoc/>
    public override async Task<List<Deck>> Get(IDictionary<string, object> searchParameters)
    {
      List<Deck> result = new List<Deck>();
      List<Tuple<Deck,int>> tuples = await Context.Set<Deck>().Select(deck => new Tuple<Deck, int>(deck, deck.Cards.Count())).ToListAsync();
      foreach (Tuple<Deck, int> tuple in tuples)
      {
        tuple.Item1.CardCount = tuple.Item2;
        result.Add(tuple.Item1);
      }
      return result;
    }

    ///<inheritdoc/>
    public override void Post(Deck entity)
    {
      UnitOfWork unitOfWork = new UnitOfWork(Context);
      unitOfWork.Execute(() => Context.Add(entity));
    }

    ///<inheritdoc/>
    public override void Put(Deck entity)
    {
      UnitOfWork unitOfWork = new UnitOfWork(Context);
      unitOfWork.Execute(() => { });
    }
  }
}