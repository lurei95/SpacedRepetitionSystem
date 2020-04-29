using Microsoft.EntityFrameworkCore;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Entities.Validation.Core;
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
    /// <param name="context">Context (injected)</param>
    /// <param name="commitValidator">CommitValidator (injected)</param>
    /// <param name="deleteValidator">DeleteValidator (injected)</param>
    public CardsController(DbContext context, DeleteValidatorBase<Card> deleteValidator, CommitValidatorBase<Card> commitValidator) 
      : base(context, deleteValidator, commitValidator) { }

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
  }
}