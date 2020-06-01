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

      foreach (CardFieldDefinition field in existing.FieldDefinitions.Where(field => !field.IsRequired))
      {
        CardFieldDefinition field1 = entity.FieldDefinitions.SingleOrDefault(x => x.FieldId == field.FieldId);
        if (field1 != null) // Update exisiting
          UpdateExistingFieldDefinition(field, field1);
        else // Remove old
          RemoveFieldDefinition(field);
      }

      //Add new
      int newId = existing.FieldDefinitions.Count + 1;
      foreach (CardFieldDefinition field in entity.FieldDefinitions.Where(x => x.FieldId == default))
      {
        AddNewFieldDefinition(existing, field, newId);
        newId++;
      }
      UpdateFieldIds(existing);
      return Ok();
    }

    ///<inheritdoc/>
    [HttpGet]
    public override async Task<ActionResult<List<CardTemplate>>> GetAsync(IDictionary<string, object> searchParameters)
    {
      IQueryable<CardTemplate> query = Context.Set<CardTemplate>()
        .Include(template => template.FieldDefinitions)
        .Where(template => template.UserId == GetUserId());
      if (searchParameters.ContainsKey(SearchTextParameter))
      {
        string searchText = searchParameters[SearchTextParameter] as string;
        query = query.Where(template => template.CardTemplateId.ToString() == searchText
          || template.Title.Contains(searchText)
          || template.FieldDefinitions.Any(field => field.FieldName.Contains(searchText)));
      }
      return await query.ToListAsync();
    }

    ///<inheritdoc/>
    protected override async Task<IActionResult> PostCoreAsync(CardTemplate entity)
    {
      UpdateFieldIds(entity);
      return await base.PostCoreAsync(entity);
    }

    private void UpdateFieldIds(CardTemplate template)
    {
      for (int i = 0; i < template.FieldDefinitions.Count; i++)
        if (template.FieldDefinitions[i].FieldId == default)
          template.FieldDefinitions[i].FieldId = i + 1;
    }

    private void UpdateExistingFieldDefinition(CardFieldDefinition fieldDefinition1, CardFieldDefinition fieldDefinition2)
    {
      fieldDefinition1.ShowInputForPractice = fieldDefinition2.ShowInputForPractice;
      if (fieldDefinition1.FieldName != fieldDefinition2.FieldName)
      {
        fieldDefinition1.FieldName = fieldDefinition2.FieldName;
        //Update fieldnames of card fields
        foreach (CardField field in Context.Set<CardField>().Where(field => field.FieldId == fieldDefinition1.FieldId))
          field.FieldName = fieldDefinition1.FieldName;
      }
    }

    private void RemoveFieldDefinition(CardFieldDefinition fieldDefinition)
    {
      Context.Entry(fieldDefinition).State = EntityState.Deleted;
      //Delete fields
      foreach (CardField field in Context.Set<CardField>().Where(field => field.FieldId == fieldDefinition.FieldId))
        Context.Entry(field).State = EntityState.Deleted;
    }

    private void AddNewFieldDefinition(CardTemplate template, CardFieldDefinition fieldDefinition, int id)
    {
      fieldDefinition.FieldId = id;
      template.FieldDefinitions.Add(fieldDefinition);
      foreach (Card card in Context.Set<Card>().Where(card => card.CardTemplateId == template.CardTemplateId))
        card.Fields.Add(new CardField() 
        { 
          CardTemplateId = template.CardTemplateId, 
          FieldName = fieldDefinition.FieldName, 
          CardId = card.CardId,
          FieldId = fieldDefinition.FieldId
        });
    }
  }
}