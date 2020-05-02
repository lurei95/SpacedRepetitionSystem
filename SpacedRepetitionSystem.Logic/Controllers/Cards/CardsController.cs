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
  /// Controller for <see cref="Card"/>
  /// </summary>
  public sealed class CardsController : EntityControllerBase<Card>
  {
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="context">Context (injected)</param>
    /// <param name="commitValidator">CommitValidator (injected)</param>
    /// <param name="deleteValidator">DeleteValidator (injected)</param>
    public CardsController(DbContext context, DeleteValidatorBase<Card> deleteValidator, CommitValidatorBase<Card> commitValidator) 
      : base(context, deleteValidator, commitValidator) { }

    ///<inheritdoc/>
    public override Card Get(object id)
    {
      return Context.Set<Card>()
        .Include(card => card.Fields)
        .Include(card => card.Deck)
        .Include(card => card.CardTemplate)
        .FirstOrDefault(card => card.CardId == (long)id);
    }

    ///<inheritdoc/>
    public override async Task<List<Card>> Get(IDictionary<string, object> searchParameters)
    {
      IQueryable<Card> query = Context.Set<Card>()
        .Include(card => card.Fields)
        .Include(card => card.Deck)
        .Include(card => card.CardTemplate);
      if (searchParameters.ContainsKey(nameof(Deck.DeckId)))
        query = query.Where(card => card.DeckId == (long)searchParameters[nameof(Deck.DeckId)]);
      return await query.ToListAsync();
    }

    ///<inheritdoc/>
    protected override void PutCore(Card entity)
    {
      CreatePracticeFields(entity);
      base.PutCore(entity);
    }

    ///<inheritdoc/>
    protected override void PostCore(Card entity)
    {
      CreatePracticeFields(entity);
      base.PostCore(entity);
    }

    private void CreatePracticeFields(Card card)
    {
      foreach (CardField field in card.Fields)
      {
        bool practiceFieldExists = Context.Set<PracticeField>()
          .Any(practiceField => practiceField.CardId == card.CardId
            && practiceField.DeckId == card.DeckId
            && practiceField.FieldName == field.FieldName);
        if (!practiceFieldExists)
        {
          PracticeField practiceField = new PracticeField()
          {
            DeckId = card.DeckId,
            FieldName = field.FieldName,
            DueDate = DateTime.Today
          };
          card.PracticeFields.Add(practiceField);
        }
      }
    }
  }
}