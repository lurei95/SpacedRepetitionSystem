using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SpacedRepetitionSystem.Entities.Entities.Cards.Configurations
{
  /// <summary>
  /// Configuration für <see cref="CardField"/>
  /// </summary>
  public sealed class CardFieldConfiguration : IEntityTypeConfiguration<CardField>
  {
    ///<inheritdoc/>
    public void Configure(EntityTypeBuilder<CardField> builder)
    {
      builder.ToTable("CardFields", "Cards");
      builder.HasKey(field => new { field.CardId, field.FieldId });

      builder.Property(field => field.CardId)
        .IsRequired();
      builder.Property(field => field.FieldId)
        .IsRequired();
      builder.Property(field => field.FieldName)
        .IsRequired();
      builder.Property(field => field.CardTemplateId)
        .IsRequired();
      builder.Property(field => field.DueDate)
        .IsRequired();
      builder.Property(field => field.ProficiencyLevel)
        .IsRequired();
      builder.Property(field => field.Value);

      builder.HasOne(field => field.CardFieldDefinition)
        .WithMany()
        .HasForeignKey(field => new { field.CardTemplateId, field.FieldId })
        .OnDelete(DeleteBehavior.Restrict);

      builder.HasOne(field => field.CardTemplate)
        .WithMany()
        .HasForeignKey(field => field.CardTemplateId)
        .OnDelete(DeleteBehavior.Restrict);

      builder.HasIndex(field => new { field.CardId, field.FieldName })
        .IsUnique();
    }
  }
}