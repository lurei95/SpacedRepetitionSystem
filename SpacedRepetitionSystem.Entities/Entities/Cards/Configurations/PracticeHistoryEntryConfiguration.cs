using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SpacedRepetitionSystem.Entities.Entities.Cards.Configurations
{
  /// <summary>
  /// Configuration for <see cref="PracticeHistoryEntry"/>
  /// </summary>
  public sealed class PracticeHistoryEntryConfiguration : IEntityTypeConfiguration<PracticeHistoryEntry>
  {
    ///<inheritdoc/>
    public void Configure(EntityTypeBuilder<PracticeHistoryEntry> builder)
    {
      builder.ToTable("PracticeHistoryEntries", "Cards");
      builder.HasKey(field => new { field.DeckId, field.CardId, field.FieldName });

      builder.Property(field => field.CardId)
        .IsRequired();
      builder.Property(field => field.FieldName)
        .IsRequired();
      builder.Property(field => field.DeckId)
        .IsRequired();
      builder.Property(field => field.PracticeDate)
        .IsRequired();
      builder.Property(field => field.CorrectCount)
        .IsRequired();
      builder.Property(field => field.HardCount)
        .IsRequired();
      builder.Property(field => field.WrongCount)
        .IsRequired();
    }
  }
}
