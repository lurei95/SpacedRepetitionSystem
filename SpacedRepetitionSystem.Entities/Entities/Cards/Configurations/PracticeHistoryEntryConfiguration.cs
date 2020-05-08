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

      builder.HasKey(entry => entry.PracticeHistoryEntryId);

      builder.Property(entry => entry.PracticeHistoryEntryId)
        .IsRequired()
        .ValueGeneratedOnAdd();
      builder.Property(entry => entry.CardId)
        .IsRequired();
      builder.Property(entry => entry.FieldName)
        .IsRequired();
      builder.Property(entry => entry.DeckId)
        .IsRequired();
      builder.Property(entry => entry.PracticeDate)
        .IsRequired();
      builder.Property(entry => entry.CorrectCount)
        .IsRequired();
      builder.Property(entry => entry.HardCount)
        .IsRequired();
      builder.Property(entry => entry.WrongCount)
        .IsRequired();

      builder.HasOne(entry => entry.Deck)
        .WithMany()
        .HasForeignKey(entry => entry.DeckId)
        .OnDelete(DeleteBehavior.Cascade);

      builder.HasOne(entry => entry.Card)
        .WithOne()
        .HasForeignKey<PracticeHistoryEntry>(entry => entry.CardId)
        .OnDelete(DeleteBehavior.Cascade);

      builder.HasOne(entry => entry.Field)
        .WithOne()
        .HasForeignKey<PracticeHistoryEntry>(entry => new { entry.CardId, entry.FieldName })
        .OnDelete(DeleteBehavior.Cascade);
    }
  }
}