﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Entities.Validation.Core;
using SpacedRepetitionSystem.WebAPI.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.WebAPI.Controllers.Cards
{
  /// <summary>
  /// Controller for <see cref="Card"/>
  /// </summary>
  [Route("[controller]")]
  [ApiController]
  public sealed class CardsController : EntityControllerBase<Card>
  {
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="commitValidator">CommitValidator (injected)</param>
    /// <param name="deleteValidator">DeleteValidator (injected)</param>
    /// <param name="context">DBContext (injected)</param>
    public CardsController(DeleteValidatorBase<Card> deleteValidator, CommitValidatorBase<Card> commitValidator, DbContext context)
      : base(deleteValidator, commitValidator, context) { }


    ///<inheritdoc/>
    [HttpGet("{id}")]
    public async override Task<ActionResult<Card>> GetAsync(object id)
    {
      Card card = await Context.Set<Card>()
        .Include(card => card.Fields)
        .Include(card => card.Deck)
        .Include(card => card.CardTemplate)
        .FirstOrDefaultAsync(card => card.CardId == (long)id);
      if (card == null)
        return NotFound();

      return card;
    }

    ///<inheritdoc/>
    [HttpGet]
    public override async Task<ActionResult<List<Card>>> GetAsync(IDictionary<string, object> searchParameters)
    {
      IQueryable<Card> query = Context.Set<Card>()
        .Include(card => card.Fields)
        .Include(card => card.Deck)
        .Include(card => card.CardTemplate);
      if (searchParameters != null && searchParameters.ContainsKey(nameof(Deck.DeckId)))
        query = query.Where(card => card.DeckId == (long)searchParameters[nameof(Deck.DeckId)]);
      return await query.ToListAsync();
    }
  }
}