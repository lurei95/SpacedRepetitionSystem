using Microsoft.EntityFrameworkCore;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Entities.Validation.Core;
using SpacedRepetitionSystem.Logic.Controllers.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpacedRepetitionSystem.Logic.Controllers.Cards
{
  /// <summary>
  /// Controller for <see cref="CardTemplate"/>
  /// </summary>
  public sealed class CardTemplatesController : EntityControllerBase<CardTemplate>
  {
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="commitValidator">CommitValidator (injected)</param>
    /// <param name="deleteValidator">DeleteValidator (injected)</param>
    public CardTemplatesController(DeleteValidatorBase<CardTemplate> deleteValidator, 
      CommitValidatorBase<CardTemplate> commitValidator)
      : base(deleteValidator, commitValidator) { }

    ///<inheritdoc/>
    public override CardTemplate Get(object id)
    {
      return Context.Set<CardTemplate>()
        .Include(definition => definition.FieldDefinitions)
        .FirstOrDefault(card => card.CardTemplateId == (long)id);
    }

    ///<inheritdoc/>
    public override async Task<List<CardTemplate>> Get(IDictionary<string, object> searchParameters)
    {
      return await Context.Set<CardTemplate>()
        .Include(definition => definition.FieldDefinitions)
        .ToListAsync();
    }
  }
}