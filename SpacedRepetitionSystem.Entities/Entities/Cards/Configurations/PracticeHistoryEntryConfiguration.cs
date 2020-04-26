using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SpacedRepetitionSystem.Entities.Entities.Cards.Configurations
{
  /// <summary>
  /// Configuration für <see cref="PracticeHistoryEntry"/>
  /// </summary>
  public sealed class PracticeHistoryEntryConfiguration : IEntityTypeConfiguration<PracticeHistoryEntry>
  {
    public void Configure(EntityTypeBuilder<PracticeHistoryEntry> builder)
    {
      builder.ToTable("PracticeHistoryEntries", "Cards");
      builder.HasKey(entry => new { entry.CardId, entry.CardFieldDefinitionId });

      builder.Property(entry => entry.CardId)
        .IsRequired();

      builder.Property(entry => entry.CardFieldDefinitionId)
        .IsRequired();

      builder.Property(entry => entry.PracticeDate)
        .IsRequired();

      builder.Property(entry => entry.PracticeResult)
        .IsRequired();
    }
  }
}