using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SpacedRepetitionSystem.Entities.Entities.Cards.Configurations
{
  /// <summary>
  /// Configuration für <see cref="Deck"/>
  /// </summary>
  public sealed class DeckConfiguration : IEntityTypeConfiguration<Deck>
  {
    ///<inheritdoc/>
    public void Configure(EntityTypeBuilder<Deck> builder)
    {
      builder.ToTable("Decks", "Cards");
      builder.HasKey(set => set.DeckId);

      builder.Property(set => set.DeckId)
        .IsRequired()
        .ValueGeneratedOnAdd();

      builder.Property(set => set.Title)
        .IsRequired()
        .HasMaxLength(100);

      builder.Property(set => set.DefaultCardTemplateId);

      builder.HasMany(set => set.Cards)
        .WithOne(card => card.Deck)
        .HasForeignKey(card => card.DeckId)
        .OnDelete(DeleteBehavior.Cascade);

      builder.HasOne(set => set.DefaultCardTemplate).WithMany()
        .HasForeignKey(set => set.DefaultCardTemplateId)
        .OnDelete(DeleteBehavior.Restrict);
    }
  }
}