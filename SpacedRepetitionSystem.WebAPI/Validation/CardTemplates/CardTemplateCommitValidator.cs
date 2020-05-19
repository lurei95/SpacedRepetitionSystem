using Microsoft.EntityFrameworkCore;
using SpacedRepetitionSystem.Entities;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Entities.Validation.CardTemplates;
using SpacedRepetitionSystem.Utility.Extensions;
using SpacedRepetitionSystem.WebAPI.Validation.Core;
using System.Linq;

namespace SpacedRepetitionSystem.WebAPI.Validation.CardTemplates
{
  /// <summary>
  /// CommitValidator for <see cref="CardTemplate"/>
  /// </summary>
  public sealed class CardTemplateCommitValidator : CommitValidatorBase<CardTemplate>
  {
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="context">Context (Injected)</param>
    public CardTemplateCommitValidator(DbContext context) : base(context) { }

    ///<inheritdoc/>
    public override string Validate(CardTemplate entity)
    {
      string error = new CardTemplateTitleValidator().Validate(entity, entity.Title);
      if (!string.IsNullOrEmpty(error))
        return error;
      if (Context.Set<CardTemplate>().Any(template => template.CardTemplateId != entity.CardTemplateId 
        && template.Title == entity.Title
        && template.UserId == entity.UserId))
        return Errors.CardTemplateTitleNotUnique.FormatWith(entity.Title);
      return null;
    }
  }
}