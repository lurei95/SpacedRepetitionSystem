using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SpacedRepetitionSystem.Entities.Entities.SmartCards.Configurations
{
  /// <summary>
  /// Configuration für <see cref="SmartCardDefinition"/>
  /// </summary>
  public sealed class SmartCardDefinitionConfiguration : IEntityTypeConfiguration<SmartCardDefinition>
  {
    ///<inheritdoc/>
    public void Configure(EntityTypeBuilder<SmartCardDefinition> builder)
    {
      builder.ToTable("SmartCardDefinition", "SmartCards");
      builder.HasKey(definition => definition.SmartCardDefinitionId);

      builder.Property(definition => definition.SmartCardDefinitionId)
        .IsRequired()
        .ValueGeneratedOnAdd();

      builder.Property(definition => definition.Title)
        .IsRequired()
        .HasMaxLength(100);

      builder.HasMany(cardDefinition => cardDefinition.FieldDefinitions)
        .WithOne(fieldDefinition => fieldDefinition.SmartCardDefinition)
        .HasForeignKey(fieldDefinition => fieldDefinition.SmartCardDefinitionId)
        .OnDelete(DeleteBehavior.Cascade);
    }
  }
}