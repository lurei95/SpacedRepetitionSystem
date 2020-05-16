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

      builder.HasKey(entry => new { entry.UserId, entry.PracticeHistoryEntryId });

      builder.Property(field => field.UserId)
        .IsRequired();
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

      builder.HasOne(entry => entry.User)
        .WithMany()
        .HasForeignKey(entry => entry.UserId)
        .OnDelete(DeleteBehavior.Cascade);

      builder.HasIndex(entry => entry.DeckId).IsUnique(false);
      builder.HasIndex(entry => entry.CardId).IsUnique(false);
    }
  }
}