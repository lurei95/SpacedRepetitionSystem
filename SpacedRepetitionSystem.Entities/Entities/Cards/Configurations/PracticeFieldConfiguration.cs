using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SpacedRepetitionSystem.Entities.Entities.Cards.Configurations
{
  /// <summary>
  /// Configuration für <see cref="PracticeFieldConfiguration"/>
  /// </summary>
  public sealed class PracticeFieldConfiguration : IEntityTypeConfiguration<PracticeField>
  {
    ///<inheritdoc/>
    public void Configure(EntityTypeBuilder<PracticeField> builder)
    {
      builder.ToTable("Cards", "PracticeFields");
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
        .HasForeignKey<CardField>(field => new { field.CardId, field.FieldName })
        .OnDelete(DeleteBehavior.Cascade);

      builder.HasOne(field => field.Deck)
        .WithMany(deck => deck.PracticeFields)
        .HasForeignKey(field => field.DeckId)
        .OnDelete(DeleteBehavior.Cascade);
    }
  }
}