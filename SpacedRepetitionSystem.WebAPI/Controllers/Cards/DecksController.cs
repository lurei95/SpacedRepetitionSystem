using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.WebAPI.Core;
using SpacedRepetitionSystem.WebAPI.Validation.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.WebAPI.Controllers.Cards
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
        .FirstOrDefaultAsync(deck1 => deck1.DeckId == id);

      if (deck == null)
        return NotFound();
      if (deck.UserId != GetUserId())
        return Unauthorized();
      return deck;
    }

    ///<inheritdoc/>
    [HttpGet]
    public override async Task<ActionResult<List<Deck>>> GetAsync(IDictionary<string, object> searchParameters)
    {
      IQueryable<Deck> query = Context.Set<Deck>()
        .Where(deck => deck.UserId == GetUserId());

      if (searchParameters != null && searchParameters.ContainsKey(nameof(Deck.IsPinned)))
        query = query.Where(deck => deck.IsPinned == (bool)searchParameters[nameof(Deck.IsPinned)]);

      List<Deck> result = await query.ToListAsync();
      foreach (Deck deck in result)
      {
        IEnumerable<CardField> fields = Context.Set<Card>()
          .AsNoTracking()
          .Where(card => card.DeckId == deck.DeckId)
          .SelectMany(card => card.Fields)
          .AsEnumerable();
        deck.CardCount = fields.Count();
        deck.DueCardCount = fields.Count(field => !string.IsNullOrEmpty(field.Value) && field.DueDate <= DateTime.Today);
      }
      return result;
    }
  }
}