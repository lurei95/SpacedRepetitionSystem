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

    /// <summary>
    /// Adds the Property validator for <see cref="Card"/>
    /// </summary>
    /// <param name="services">Service-collection</param>
    public static void AddCardPropertyValidator(this IServiceCollection services)
    {
      EntityChangeValidator<Card> validator = new EntityChangeValidator<Card>();
      validator.Register(nameof(Card.CardTemplateId), new CardCardTemplateIdValidator());
      validator.Register(nameof(Card.DeckId), new CardDeckIdValidator());
      services.AddSingleton(typeof(EntityChangeValidator<Card>), validator);
    }

    /// <summary>
    /// Adds the Property validator for <see cref="Deck"/>
    /// </summary>
    /// <param name="services">Service-collection</param>
    public static void AddDecksPropertyValidator(this IServiceCollection services)
    {
      EntityChangeValidator<Deck> validator = new EntityChangeValidator<Deck>();
      services.AddSingleton(typeof(EntityChangeValidator<Deck>), validator);
    }
  }
}