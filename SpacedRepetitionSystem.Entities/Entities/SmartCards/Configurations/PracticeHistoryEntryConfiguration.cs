using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SpacedRepetitionSystem.Entities.Entities.SmartCards.Configurations
{
  /// <summary>
  /// Configuration für <see cref="PracticeHistoryEntry"/>
  /// </summary>
  public sealed class PracticeHistoryEntryConfiguration : IEntityTypeConfiguration<PracticeHistoryEntry>
  {
    public void Configure(EntityTypeBuilder<PracticeHistoryEntry> builder)
    {
      builder.ToTable("PracticeHistoryEntries", "SmartCards");
      builder.HasKey(entry => new { entry.SmartCardId, entry.SmartCardFieldDefinitionId });

      builder.Property(entry => entry.SmartCardId)
        .IsRequired();

      builder.Property(entry => entry.SmartCardFieldDefinitionId)
        .IsRequired();

      builder.Property(entry => entry.PracticeDate)
        .IsRequired();

      builder.Property(entry => entry.PracticeResult)
        .IsRequired();
    }
  }
}