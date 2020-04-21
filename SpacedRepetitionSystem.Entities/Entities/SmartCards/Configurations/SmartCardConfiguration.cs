using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SpacedRepetitionSystem.Entities.Entities.SmartCards.Configurations
{
  /// <summary>
  /// Configuration für <see cref="SmartCard"/>
  /// </summary>
  public sealed class SmartCardConfiguration : IEntityTypeConfiguration<SmartCard>
  {
    ///<inheritdoc/>
    public void Configure(EntityTypeBuilder<SmartCard> builder)
    {
      builder.ToTable("SmartCards", "SmartCards");
      builder.HasKey(card => card.SmartCardId);

      builder.Property(card => card.SmartCardId)
        .IsRequired()
        .ValueGeneratedOnAdd();

      builder.Property(card => card.Tags)
        .IsRequired()
        .HasMaxLength(256);

      builder.Property(card => card.PracticeSetId)
        .IsRequired();

      builder.HasOne(card => card.SmartCardDefinition)
        .WithMany()
        .HasForeignKey(card => card.SmartCardDefinitionId)
        .OnDelete(DeleteBehavior.Restrict);

      builder.HasMany(card => card.Fields)
        .WithOne(field => field.SmartCard)
        .HasForeignKey(field => field.SmartCardId)
        .OnDelete(DeleteBehavior.Cascade);
    }
  }
}