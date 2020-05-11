using Microsoft.EntityFrameworkCore;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Entities.Validation.Core;
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
    /// <param name="commitValidator">CommitValidator (injected)</param>
    /// <param name="deleteValidator">DeleteValidator (injected)</param>
    public DecksController(DeleteValidatorBase<Deck> deleteValidator,
      CommitValidatorBase<Deck> commitValidator)
      : base(deleteValidator, commitValidator) { }

    ///<inheritdoc/>
    public override Deck Get(object id)
    {
      return Context.Set<Deck>()
        .Include(deck => deck.Cards)
        .ThenInclude(card => card.Fields)
        .FirstOrDefault(card => card.DeckId == (long)id);
    }

    ///<inheritdoc/>
    public override async Task<List<Deck>> Get(IDictionary<string, object> searchParameters)
    {
      List<Deck> result = new List<Deck>();
      IQueryable<Deck> query = Context.Set<Deck>();
      if (searchParameters != null && searchParameters.ContainsKey(nameof(Deck.IsPinned)))
        query = query.Where(deck => deck.IsPinned == (bool)searchParameters[nameof(Deck.IsPinned)]);

      List<Tuple<Deck, int, int>> tuples = await query.Select(
          deck => new Tuple<Deck, int, int>(deck, deck.Cards.Count(), 
          deck.Cards.SelectMany(card => card.Fields).Count(field => field.DueDate <= DateTime.Today)))
        .ToListAsync();
      foreach (Tuple<Deck, int, int> tuple in tuples)
      {
        tuple.Item1.CardCount = tuple.Item2;
        tuple.Item1.DueCardCount = tuple.Item3;
        result.Add(tuple.Item1);
      }
      return result;
    }
  }
}