using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SpacedRepetitionSystem.Entities.Entities.Cards.Configurations
{
  /// <summary>
  /// Configuration für <see cref="Card"/>
  /// </summary>
  public sealed class CardConfiguration : IEntityTypeConfiguration<Card>
  {
    ///<inheritdoc/>
    public void Configure(EntityTypeBuilder<Card> builder)
    {
      builder.ToTable("Cards", "Cards");
      builder.HasKey(card => card.CardId);

      builder.Property(card => card.CardId)
        .IsRequired()
        .ValueGeneratedOnAdd();

      builder.Property(card => card.DueDate)
        .IsRequired();

      builder.Property(card => card.Tags)
        .IsRequired()
        .HasMaxLength(256);

      builder.Property(card => card.DeckId)
        .IsRequired();

      builder.HasOne(card => card.CardTemplate)
        .WithMany()
        .HasForeignKey(card => card.CardTemplateId)
        .OnDelete(DeleteBehavior.Restrict);

      builder.HasMany(card => card.Fields)
        .WithOne(field => field.Card)
        .HasForeignKey(field => field.CardId)
        .OnDelete(DeleteBehavior.Cascade);
    }
  }
}