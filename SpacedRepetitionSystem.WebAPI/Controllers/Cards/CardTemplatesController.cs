using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
      : base(deleteValidator, commitValidator, context) { }

    ///<inheritdoc/>
    [HttpGet("{id}")]
    public override async Task<ActionResult<CardTemplate>> GetAsync([FromRoute] long id)
    {
      CardTemplate template = await Context.Set<CardTemplate>()
        .Include(definition => definition.FieldDefinitions)
        .FirstOrDefaultAsync(template1 => template1.UserId == GetUserId() && template1.CardTemplateId == (long)id);
      if (template == null)
        return NotFound();
      return template;
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