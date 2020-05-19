using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.WebAPI.Core;
using SpacedRepetitionSystem.WebAPI.Validation.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.WebAPI.Controllers.Cards
{
  /// <summary>
  /// Controller for <see cref="Card"/>
  /// </summary>
  [Authorize]
  [Route("[controller]")]
  [ApiController]
  public sealed class CardsController : EntityControllerBase<Card, long>
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
    public async override Task<ActionResult<Card>> GetAsync([FromRoute] long id)
    {
      Card card = await Context.Set<Card>()
        .Include(card => card.Fields)
        .Include(card => card.Deck)
        .Include(card => card.CardTemplate)
        .FirstOrDefaultAsync(card => card.CardId == id);

      if (card == null)
        return NotFound();
      if (card.UserId != GetUserId())
        return Unauthorized();
      return card;
    }

    ///<inheritdoc/>
    [HttpGet]
    public override async Task<ActionResult<List<Card>>> GetAsync(IDictionary<string, object> searchParameters)
    {
      IQueryable<Card> query = Context.Set<Card>()
        .Include(card => card.Fields)
        .Include(card => card.Deck)
        .Include(card => card.CardTemplate)
        .Where(card => card.UserId == GetUserId());
      if (searchParameters != null && searchParameters.ContainsKey(nameof(Deck.DeckId)))
        query = query.Where(card => card.DeckId == (long)searchParameters[nameof(Deck.DeckId)]);
      return await query.ToListAsync();
    }

    ///<inheritdoc/>
    protected override async Task<IActionResult> PutCoreAsync(Card entity)
    {
      Card existing = await Context.Set<Card>()
        .Include(card => card.Fields)
        .FirstOrDefaultAsync(card => card.CardId == entity.CardId);
      if (existing == null)
        return NotFound();
      existing.CardTemplateId = entity.CardTemplateId;
      existing.Tags = entity.Tags;

      foreach (CardField field in existing.Fields)
      {
        CardField field1 = entity.Fields.SingleOrDefault(x => x.FieldName == field.FieldName);
        if (field1 != null)
        {
          field.CardTemplateId = field1.CardTemplateId;
          field.DueDate = field1.DueDate;
          field.Value = field1.Value;
          field.ProficiencyLevel = field1.ProficiencyLevel;
        }
        else
          Context.Entry(field).State = EntityState.Deleted;
      }

      foreach (CardField field in entity.Fields.Where(x => !existing.Fields.Any(y => y.FieldName == x.FieldName)))
        existing.Fields.Add(field);
      return Ok();
    }
  }
}