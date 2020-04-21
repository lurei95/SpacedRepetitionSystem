using SpacedRepetitionSystem.Entities.Entities;

namespace SpacedRepetitionSystem.Entities.Validation.Core
{
  /// <summary>
  /// Baseclass for a validator that validates the chnage of a property value 
  /// </summary>
  /// <typeparam name="TEntity">Entity-type</typeparam>
  /// <typeparam name="TProperty">Property-Type</typeparam>
  public abstract class PropertyValidatorBase<TEntity, TProperty> : IPropertyValidator where TEntity : IEntity
  {
    ///<inheritdoc>
    public abstract string PropertyName { get; }

    /// <summary>
    /// Validates the property change
    /// </summary>
    /// <param name="entity">The entity</param>
    /// <param name="newValue">New value of the property</param>
    /// <returns></returns>
    public abstract string Validate(TEntity entity, TProperty newValue);
  }
}