﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Entities.Validation.Core;
using SpacedRepetitionSystem.WebAPI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Logic.Controllers.Cards
{
  /// <summary>
  /// Controller for <see cref="Deck"/>
  /// </summary>
  [Authorize]
  [Route("[controller]")]
  [ApiController]
  public sealed class DecksController : EntityControllerBase<Deck, long>
  {
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="commitValidator">CommitValidator (injected)</param>
    /// <param name="deleteValidator">DeleteValidator (injected)</param>
    /// <param name="context">DBContext (injected)</param>
    public DecksController(DeleteValidatorBase<Deck> deleteValidator, CommitValidatorBase<Deck> commitValidator, DbContext context)
      : base(deleteValidator, commitValidator, context) { }

    ///<inheritdoc/>
    [HttpGet("{id}")]
    public override async Task<ActionResult<Deck>> GetAsync([FromRoute] long id)
    {
      Deck deck = await Context.Set<Deck>()
        .Include(deck => deck.Cards)
        .ThenInclude(card => card.Fields)
        .ThenInclude(field => field.CardFieldDefinition)
        .FirstOrDefaultAsync(deck1 => deck1.UserId == GetUserId() && deck1.DeckId == (long)id);
      if (deck == null)
        return NotFound();
      return deck;
    }

    ///<inheritdoc/>
    [HttpGet]
    public override async Task<ActionResult<List<Deck>>> GetAsync(IDictionary<string, object> searchParameters)
    {
      List<Deck> result = new List<Deck>();
      IQueryable<Deck> query = Context.Set<Deck>()
        .Where(deck => deck.UserId == GetUserId());
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