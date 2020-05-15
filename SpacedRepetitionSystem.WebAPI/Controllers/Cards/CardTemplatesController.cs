﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Entities.Validation.Core;
using SpacedRepetitionSystem.WebAPI.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.WebAPI.Controllers.Cards
{
  /// <summary>
  /// Controller for <see cref="CardTemplate"/>
  /// </summary>
  [Route("[controller]")]
  [ApiController]
  public sealed class CardTemplatesController : EntityControllerBase<CardTemplate>
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
    public override async Task<ActionResult<CardTemplate>> GetAsync(object id)
    {
      CardTemplate template = await Context.Set<CardTemplate>()
        .Include(definition => definition.FieldDefinitions)
        .FirstOrDefaultAsync(card => card.CardTemplateId == (long)id);
      if (template == null)
        return NotFound();
      return template;
    }

    ///<inheritdoc/>
    public override async Task<ActionResult<List<CardTemplate>>> GetAsync(IDictionary<string, object> searchParameters)
    {
      return await Context.Set<CardTemplate>()
        .Include(definition => definition.FieldDefinitions)
        .ToListAsync();
    }
  }
}