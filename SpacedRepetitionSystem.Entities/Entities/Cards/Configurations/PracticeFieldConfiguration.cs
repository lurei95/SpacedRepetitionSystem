using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SpacedRepetitionSystem.Entities.Entities.Cards.Configurations
{
  /// <summary>
  /// Configuration for <see cref="PracticeField"/>
  /// </summary>
  public sealed class PracticeFieldConfiguration : IEntityTypeConfiguration<PracticeField>
  {
    ///<inheritdoc/>
    public void Configure(EntityTypeBuilder<PracticeField> builder)
    {
      builder.ToTable("PracticeFields", "Cards");
      builder.HasKey(field => new { field.DeckId, field.CardId, field.FieldName });

      builder.Property(field => field.CardId)
        .IsRequired();
      builder.Property(field => field.FieldName)
        .IsRequired();
      builder.Property(field => field.DeckId)
        .IsRequired();
      builder.Property(field => field.DueDate)
        .IsRequired();

      builder.HasOne(field => field.Field)
        .WithOne()
        .HasForeignKey<PracticeField>(field => new { field.CardId, field.FieldName })
        .OnDelete(DeleteBehavior.Cascade);
    }
  }
}