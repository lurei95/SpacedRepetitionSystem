using Microsoft.Extensions.DependencyInjection;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Entities.Validation.Core;
using SpacedRepetitionSystem.Entities.Validation.Cards;

namespace SpacedRepetitionSystem.Entities
{
  /// <summary>
  /// Class containing extensions for DI
  /// </summary>
  public static class DependencyInjectionExtensions
  {
    /// <summary>
    /// Adds a valdiation service
    /// </summary>
    /// <typeparam name="TValidator">Base type</typeparam>
    /// <typeparam name="TImplementation">Implementation</typeparam>
    /// <param name="services">service collection</param>
    public static void AddValidator<TValidator, TImplementation>(this IServiceCollection services)
      where TValidator : class where TImplementation : class, TValidator
      => services.AddSingleton<TValidator, TImplementation>();

    public static void AddCardPropertyValidator(this IServiceCollection services)
    {
      EntityChangeValidator<Card> validator = new EntityChangeValidator<Card>();
      validator.Register(nameof(Card.CardTemplateId), new CardCardTemplateIdValidator());
      validator.Register(nameof(Card.DeckId), new CardDeckIdValidator());
      services.AddSingleton(typeof(EntityChangeValidator<Card>), validator);
    }
  }
}