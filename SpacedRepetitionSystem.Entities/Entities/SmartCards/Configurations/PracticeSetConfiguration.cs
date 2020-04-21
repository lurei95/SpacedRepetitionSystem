using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SpacedRepetitionSystem.Entities.Entities.SmartCards.Configurations
{
  /// <summary>
  /// Configuration für <see cref="PracticeSet"/>
  /// </summary>
  public sealed class PracticeSetConfiguration : IEntityTypeConfiguration<PracticeSet>
  {
    ///<inheritdoc/>
    public void Configure(EntityTypeBuilder<PracticeSet> builder)
    {
      builder.ToTable("PracticeSets", "SmartCards");
      builder.HasKey(set => set.PracticeSetId);

      builder.Property(set => set.PracticeSetId)
        .IsRequired()
        .ValueGeneratedOnAdd();

      builder.Property(set => set.Title)
        .IsRequired()
        .HasMaxLength(100);

      builder.Property(set => set.DefaultSmartCardDefinitionId);

      builder.HasMany(set => set.SmartCards)
        .WithOne(card => card.PracticeSet)
        .HasForeignKey(card => card.PracticeSetId)
        .OnDelete(DeleteBehavior.Cascade);

      builder.HasOne(set => set.DefaultSmartCardDefinition).WithMany()
        .HasForeignKey(set => set.DefaultSmartCardDefinitionId)
        .OnDelete(DeleteBehavior.Restrict);
    }
  }
}