using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Entities.Validation.Core;
using SpacedRepetitionSystem.WebAPI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.WebAPI.Controllers.Cards
{
  /// <summary>
  /// Controller for <see cref="PracticeHistoryEntry"/>
  /// </summary>
  [Authorize]
  [Route("[controller]")]
  [ApiController]
  public sealed class PracticeHistoryEntriesController : EntityControllerBase<PracticeHistoryEntry, long>
  {
    private static Dictionary<int, int> ProficiencyDueDaysLookup { get; } = new Dictionary<int, int>()
    {
      { 1, 1 },
      { 2, 2 },
      { 3, 7 },
      { 4, 14 },
      { 5, 30 },
      { 6, 90 }
    };

    /// <summary>
    /// Search parameter for the top 10 problem words
    /// </summary>
    public static readonly string ProblemWords = nameof(ProblemWords);

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="commitValidator">CommitValidator (injected)</param>
    /// <param name="deleteValidator">DeleteValidator (injected)</param>
    /// <param name="context">DBContext (injected)</param>
    public PracticeHistoryEntriesController(DeleteValidatorBase<PracticeHistoryEntry> deleteValidator, 
      CommitValidatorBase<PracticeHistoryEntry> commitValidator, DbContext context)
      : base(deleteValidator, commitValidator, context) { }

    ///<inheritdoc/>
    [HttpGet("{id}")]
    public override async Task<ActionResult<PracticeHistoryEntry>> GetAsync([FromRoute] long id)
    {
      PracticeHistoryEntry entry = await Context.Set<PracticeHistoryEntry>()
        .FirstOrDefaultAsync(entry1 => entry1.PracticeHistoryEntryId == id);

      if (entry == null)
        return NotFound();
      if (entry.UserId != GetUserId())
        return Unauthorized();
      return entry;
    }

    ///<inheritdoc/>
    [HttpGet]
    public override async Task<ActionResult<List<PracticeHistoryEntry>>> GetAsync(IDictionary<string, object> searchParameters)
    {
      IQueryable<PracticeHistoryEntry> results = Context.Set<PracticeHistoryEntry>()
        .Where(entry => entry.UserId == GetUserId());

      if (searchParameters.ContainsKey(ProblemWords))
      {
        results = results.GroupBy(entry => new { entry.DeckId, entry.CardId }, (x, y) => new PracticeHistoryEntry()
        {
          CorrectCount = y.Sum(z => z.CorrectCount),
          HardCount = y.Sum(z => z.HardCount),
          WrongCount = y.Sum(z => z.WrongCount),
          CardId = x.CardId,
          DeckId = x.DeckId
        })
          .OrderBy(entry => (entry.CorrectCount == 0 && entry.WrongCount == 0) ? 1 : (double)entry.CorrectCount / (entry.WrongCount + entry.CorrectCount))
          .Take(10);
      }

      if (searchParameters != null && searchParameters.ContainsKey(nameof(Card.DeckId)))
        results = results.Where(entry => entry.DeckId == (long)searchParameters[nameof(Card.DeckId)]);
      if (searchParameters != null && searchParameters.ContainsKey(nameof(Card.CardId)))
        results = results.Where(entry => entry.CardId == (long)searchParameters[nameof(Card.CardId)]);
      if (searchParameters != null && searchParameters.ContainsKey(nameof(CardField.FieldName)))
        results = results.Where(entry => entry.FieldName == (searchParameters[nameof(CardField.FieldName)] as string));
      return await results.ToListAsync();
    }

    ///<inheritdoc/>
    protected override async Task<IActionResult> PostCoreAsync(PracticeHistoryEntry entity)
    {
      if (entity == null)
        return BadRequest();

      PracticeHistoryEntry existingEntry = await Context.Set<PracticeHistoryEntry>()
      .Where(entry => entry.PracticeDate.Date == entity.PracticeDate.Date
        && entry.DeckId == entity.DeckId && entry.CardId == entity.CardId
        && entry.FieldName == entity.FieldName).FirstOrDefaultAsync();

      if (existingEntry != null)
      {
        existingEntry.CorrectCount += entity.CorrectCount;
        existingEntry.HardCount += entity.HardCount;
        existingEntry.WrongCount += entity.WrongCount;
        await UpdatePracticeInformationAsync(existingEntry);
      }
      else
      {
        await UpdatePracticeInformationAsync(entity);
        Context.Add(entity);
      }
      return Ok();
    }

    private async Task UpdatePracticeInformationAsync(PracticeHistoryEntry entry)
    {
      CardField field = await Context.Set<CardField>().FindAsync(entry.CardId, entry.FieldName);
      if (entry.WrongCount > 0)
        field.ProficiencyLevel = 1;
      else if (entry.HardCount > 0 && field.ProficiencyLevel != 1)
        field.ProficiencyLevel--;
      else if (entry.CorrectCount > 0 && field.ProficiencyLevel != 6)
        field.ProficiencyLevel++;
      field.DueDate = entry.PracticeDate.AddDays(ProficiencyDueDaysLookup[field.ProficiencyLevel]);
    }

    ///<inheritdoc/>
    protected override Task<IActionResult> DeleteCoreAsync(PracticeHistoryEntry entity)
    { throw new NotSupportedException(); }

    ///<inheritdoc/>
    protected override Task<IActionResult> PutCoreAsync(PracticeHistoryEntry entity)
    { throw new NotSupportedException(); }
  }
}