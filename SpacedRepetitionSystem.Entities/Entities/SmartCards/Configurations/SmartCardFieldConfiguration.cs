using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SpacedRepetitionSystem.Entities.Entities.SmartCards.Configurations
{
  /// <summary>
  /// Configuration für <see cref="SmartCardField"/>
  /// </summary>
  public sealed class SmartCardFieldConfiguration : IEntityTypeConfiguration<SmartCardField>
  {
    ///<inheritdoc/>
    public void Configure(EntityTypeBuilder<SmartCardField> builder)
    {
      builder.ToTable("SmartCardFields", "SmartCards");
      builder.HasKey(field => new { field.SmartCardId, field.FieldName });

      builder.Property(field => field.SmartCardId)
        .IsRequired();

      builder.Property(field => field.FieldName)
        .IsRequired();

      builder.Property(field => field.SmartCardDefinitionId)
        .IsRequired();

      builder.Property(field => field.Value);

      builder.HasOne(field => field.SmartCardFieldDefinition)
        .WithMany()
        .HasForeignKey(field => new { field.SmartCardDefinitionId, field.FieldName })
        .OnDelete(DeleteBehavior.Restrict);

      builder.HasOne(field => field.SmartCardDefinition)
        .WithMany()
        .HasForeignKey(field => field.SmartCardDefinitionId)
        .OnDelete(DeleteBehavior.Restrict);
    }
  }
}