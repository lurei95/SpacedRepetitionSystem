using Microsoft.EntityFrameworkCore;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Entities.Validation.Core;
using SpacedRepetitionSystem.Logic.Controllers.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Logic.Controllers.Cards
{
  /// <summary>
  /// Controller for <see cref="PracticeHistoryEntry"/>
  /// </summary>
  public sealed class PracticeHistoryEntriesController : EntityControllerBase<PracticeHistoryEntry>
  {
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="commitValidator">CommitValidator (injected)</param>
    /// <param name="deleteValidator">DeleteValidator (injected)</param>
    public PracticeHistoryEntriesController(DeleteValidatorBase<PracticeHistoryEntry> deleteValidator,
      CommitValidatorBase<PracticeHistoryEntry> commitValidator)
      : base(deleteValidator, commitValidator) { }

    ///<inheritdoc/>
    public override PracticeHistoryEntry Get(object id)
    { return Context.Set<PracticeHistoryEntry>().Find(id); }

    ///<inheritdoc/>
    public override async Task<List<PracticeHistoryEntry>> Get(IDictionary<string, object> searchParameters)
    {
      IQueryable<PracticeHistoryEntry> results = Context.Set<PracticeHistoryEntry>();
      if (searchParameters.ContainsKey(nameof(Card.DeckId)))
        results = results.Where(entry => entry.DeckId == (long)searchParameters[nameof(Card.DeckId)]);
      if (searchParameters.ContainsKey(nameof(Card.CardId)))
        results = results.Where(entry => entry.CardId == (long)searchParameters[nameof(Card.CardId)]);
      if (searchParameters.ContainsKey(nameof(CardField.FieldName)))
        results = results.Where(entry => entry.FieldName == (searchParameters[nameof(CardField.FieldName)] as string));
      return await results.ToListAsync();
    }

    ///<inheritdoc/>
    protected override void PostCore(PracticeHistoryEntry entity)
    {
      PracticeHistoryEntry existingEntry = Context.Set<PracticeHistoryEntry>()
        .AsNoTracking()
        .Where(entry => entry.PracticeDate.Date == entity.PracticeDate.Date
          && entry.DeckId == entity.DeckId && entry.CardId == entity.CardId
          && entry.FieldName == entity.FieldName).FirstOrDefault();
      if (existingEntry != null)
      {
        existingEntry.CorrectCount += entity.CorrectCount;
        existingEntry.HardCount += entity.HardCount;
        existingEntry.WrongCount += entity.WrongCount;
      }
      else
        Context.Add(entity);
    }

    ///<inheritdoc/>
    protected override void DeleteCore(PracticeHistoryEntry entity)
    { throw new NotSupportedException(); }

    ///<inheritdoc/>
    protected override void PutCore(PracticeHistoryEntry entity)
    { throw new NotSupportedException(); }
  }
}