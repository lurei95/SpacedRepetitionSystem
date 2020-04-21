using SpacedRepetitionSystem.Entities.Entities;
using System.Collections.Generic;

namespace SpacedRepetitionSystem.Entities.Validation.Core
{
  /// <summary>
  /// Validator that validates the change of a proeprty of an entity
  /// </summary>
  /// <typeparam name="TEntity">Entity-Type</typeparam>
  public sealed class EntityChangeValidator<TEntity> where TEntity : IEntity
  {
    private Dictionary<string, IPropertyValidator> PropertyValidators { get; } 
      = new Dictionary<string, IPropertyValidator>();

    /// <summary>
    /// Registers a property validator
    /// </summary>
    /// <param name="propertyName">name of the property</param>
    /// <param name="validator">The validator</param>
    public void Register(string propertyName, IPropertyValidator validator) 
      => PropertyValidators.Add(propertyName, validator);

    /// <summary>
    /// Validates the new value of the property
    /// </summary>
    /// <typeparam name="TProperty">Proeprty-Type</typeparam>
    /// <param name="propertyName">Name of the property</param>
    /// <param name="entity">The entity</param>
    /// <param name="newValue">The new value of the property</param>
    /// <returns>Error message or null</returns>
    public string Validate<TProperty>(string propertyName, TEntity entity, TProperty newValue)
    {
      if (PropertyValidators.ContainsKey(propertyName))
        return (PropertyValidators[propertyName] as PropertyValidatorBase<TEntity, TProperty>).Validate(entity, newValue);
      return null;
    }
  }
}