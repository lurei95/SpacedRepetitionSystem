using SpacedRepetitionSystem.Entities.Entities;
using System.Reflection;

namespace SpacedRepetitionSystem.Entities
{
  /// <summary>
  /// Helper class for geting the name of an entity
  /// </summary>
  public static class EntityNameHelper
  {
    /// <summary>
    /// Gets the localized name for an entity type
    /// </summary>
    /// <typeparam name="TEntity">the entity type</typeparam>
    /// <returns>The localized name for an entity type</returns>
    public static string GetName<TEntity>() where TEntity : IEntity
    {
      string name = null;
      PropertyInfo property = typeof(EntityNames).GetProperty(typeof(TEntity).Name);
      if (property != null)
        name = property.GetValue(null, null) as string;
      return name;
    }

    /// <summary>
    /// Gets the localized name in plural for an entity type
    /// </summary>
    /// <typeparam name="TEntity">the entity type</typeparam>
    /// <returns>The localized name for an entity type</returns>
    public static string GetPluralName<TEntity>() where TEntity : IEntity
    {
      string name = null;
      PropertyInfo property = typeof(EntityNames).GetProperty(typeof(TEntity).Name + "_Plural");
      if (property != null)
        name = property.GetValue(null, null) as string;
      return name;
    }
  }
}