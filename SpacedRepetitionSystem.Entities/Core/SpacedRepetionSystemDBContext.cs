using Microsoft.EntityFrameworkCore;
using SpacedRepetitionSystem.Entities.Entities.Cards;
using SpacedRepetitionSystem.Entities.Entities.Cards.Configurations;

namespace SpacedRepetitionSystem.Entities.Core
{
  /// <summary>
  /// The Database Context for this app
  /// </summary>
  public sealed class SpacedRepetionSystemDBContext : DbContext 
  {
    /// <summary>
    /// Cards
    /// </summary>
    public DbSet<Card> Cards { get; set; }

    /// <summary>
    /// decks
    /// </summary>
    public DbSet<Deck> Decks { get; set; }

    /// <summary>
    /// CardTemplates
    /// </summary>
    public DbSet<CardTemplate> CardTemplates { get; set; }

    /// <summary>
    /// PracticeHistoryEntries 
    /// </summary>
    public DbSet<PracticeHistoryEntry> PracticeHistoryEntries { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="options">The options</param>
    public SpacedRepetionSystemDBContext(DbContextOptions options) : base(options) { }

    ///<inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);
      modelBuilder.ApplyConfiguration(new PracticeHistoryEntryConfiguration());
      modelBuilder.ApplyConfiguration(new DeckConfiguration());
      modelBuilder.ApplyConfiguration(new CardConfiguration());
      modelBuilder.ApplyConfiguration(new CardTemplateConfiguration());
      modelBuilder.ApplyConfiguration(new CardFieldConfiguration());
      modelBuilder.ApplyConfiguration(new CardFieldDefinitionConfiguration());
    }
  }
}