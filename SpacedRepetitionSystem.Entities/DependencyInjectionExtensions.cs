using Microsoft.Extensions.DependencyInjection;
using SpacedRepetitionSystem.Entities.Entities.SmartCards;
using SpacedRepetitionSystem.Entities.Validation.Core;
using SpacedRepetitionSystem.Entities.Validation.SmartCards;

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

    public static void AddSmartCardPropertyValidator(this IServiceCollection services)
    {
      EntityChangeValidator <SmartCard> validator = new EntityChangeValidator<SmartCard>();
      validator.Register(nameof(SmartCard.SmartCardDefinitionId), new SmartCardSmartCardDefinitionIdValidator());
      validator.Register(nameof(SmartCard.PracticeSetId), new SmartCardPracticeSetIdValidator());
      services.AddSingleton(typeof(EntityChangeValidator<SmartCard>), validator);
    }
  }
}