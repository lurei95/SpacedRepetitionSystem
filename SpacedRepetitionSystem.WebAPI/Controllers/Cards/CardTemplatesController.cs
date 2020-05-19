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
  /// Controller for <see cref="CardTemplate"/>
  /// </summary>
  [Authorize]
  [Route("[controller]")]
  [ApiController]
  public sealed class CardTemplatesController : EntityControllerBase<CardTemplate, long>
  {
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="commitValidator">CommitValidator (injected)</param>
    /// <param name="deleteValidator">DeleteValidator (injected)</param>
    /// <param name="context">DBContext (injected)</param>
    public CardTemplatesController(DeleteValidatorBase<CardTemplate> deleteValidator, CommitValidatorBase<CardTemplate> commitValidator, DbContext context)
      : base(deleteValidator, commitValidator, context)
    { }

    ///<inheritdoc/>
    [HttpGet("{id}")]
    public override async Task<ActionResult<CardTemplate>> GetAsync([FromRoute] long id)
    {
      CardTemplate template = await Context.Set<CardTemplate>()
        .Include(definition => definition.FieldDefinitions)
        .FirstOrDefaultAsync(template1 => template1.CardTemplateId == id);

      if (template == null)
        return NotFound();
      if (template.UserId != GetUserId())
        return Unauthorized();
      return template;
    }

    ///<inheritdoc/>
    protected override async Task<IActionResult> PutCoreAsync(CardTemplate entity)
    {
      CardTemplate existing = await Context.Set<CardTemplate>()
        .Include(template => template.FieldDefinitions)
        .FirstOrDefaultAsync(template => template.CardTemplateId == entity.CardTemplateId);
      if (existing == null)
        return NotFound();
      existing.Title = entity.Title;

      foreach (CardFieldDefinition field in existing.FieldDefinitions)
      {
        CardFieldDefinition field1 = entity.FieldDefinitions.SingleOrDefault(x => x.FieldName == field.FieldName);
        if (field1 != null) // Update exisiting
          field1.ShowInputForPractice = field.ShowInputForPractice;
        else // Remove old
          Context.Entry(field).State = EntityState.Deleted;
      }

      //Add new
      foreach (CardFieldDefinition field in entity.FieldDefinitions.Where(x => !existing.FieldDefinitions.Any(y => y.FieldName == x.FieldName)))
      {
        existing.FieldDefinitions.Add(field);
        foreach (Card card in Context.Set<Card>().Where(card => card.CardTemplateId == existing.CardTemplateId))
          card.Fields.Add(new CardField() { CardTemplateId = existing.CardTemplateId, FieldName = field.FieldName, CardId = card.CardId });
      }
      return Ok();
    }

    ///<inheritdoc/>
    [HttpGet]
    public override async Task<ActionResult<List<CardTemplate>>> GetAsync(IDictionary<string, object> searchParameters)
    {
      return await Context.Set<CardTemplate>()
        .Include(template => template.FieldDefinitions)
        .Where(template => template.UserId == GetUserId())
        .ToListAsync();
    }
  }
}