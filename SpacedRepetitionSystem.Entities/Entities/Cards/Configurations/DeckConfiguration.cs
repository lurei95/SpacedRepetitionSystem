using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpacedRepetitionSystem.Entities.Entities.Security;

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
      builder.HasKey(deck => new { deck.UserId, deck.DeckId });

      builder.Ignore(deck => deck.CardCount);
      builder.Ignore(deck => deck.DueCardCount);

      builder.Property(deck => deck.DeckId)
        .IsRequired()
        .ValueGeneratedOnAdd();

      builder.Property(deck => deck.UserId)
        .IsRequired();

      builder.Property(deck => deck.IsPinned)
        .IsRequired();

      builder.Property(deck => deck.Title)
        .IsRequired()
        .HasMaxLength(100);

      builder.Property(deck => deck.DefaultCardTemplateId);

      builder.HasMany(deck => deck.Cards)
        .WithOne(card => card.Deck)
        .HasForeignKey(card => card.DeckId)
        .OnDelete(DeleteBehavior.Cascade);

      builder.HasOne(deck => deck.DefaultCardTemplate)
        .WithMany()
        .HasForeignKey(deck => deck.DefaultCardTemplateId)
        .OnDelete(DeleteBehavior.Restrict);

      builder.HasOne(deck => deck.User)
        .WithMany()
        .HasForeignKey(deck => deck.UserId)
        .OnDelete(DeleteBehavior.Cascade);
    }
  }
}