using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SpacedRepetitionSystem.Entities.Entities.SmartCards.Configurations
{
  /// <summary>
  /// Configuration für <see cref="SmartCardFieldDefinition"/>
  /// </summary>
  public sealed class SmartCardFieldDefinitionConfiguration : IEntityTypeConfiguration<SmartCardFieldDefinition>
  {
    ///<inheritdoc/>
    public void Configure(EntityTypeBuilder<SmartCardFieldDefinition> builder)
    {
      builder.ToTable("SmartCardFieldDefinitions", "SmartCards");
      builder.HasKey(definition => new { definition.SmartCardDefinitionId, definition.FieldName });

      builder.Property(definition => definition.FieldName)
        .IsRequired()
        .HasMaxLength(100);

      builder.Property(definition => definition.SmartCardDefinitionId)
        .IsRequired();
    }
  }
}