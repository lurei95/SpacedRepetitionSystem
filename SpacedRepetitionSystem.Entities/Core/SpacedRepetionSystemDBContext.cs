using Microsoft.EntityFrameworkCore;
using SpacedRepetitionSystem.Entities.Entities.SmartCards;
using SpacedRepetitionSystem.Entities.Entities.SmartCards.Configurations;

namespace SpacedRepetitionSystem.Entities.Core
{
  /// <summary>
  /// The Database Context for this app
  /// </summary>
  public sealed class SpacedRepetionSystemDBContext : DbContext 
  {
    /// <summary>
    /// SmartCards
    /// </summary>
    public DbSet<SmartCard> SmartCards { get; set; }

    /// <summary>
    /// PracticeSets
    /// </summary>
    public DbSet<PracticeSet> PracticeSets { get; set; }

    /// <summary>
    /// SmartCardDefinitions
    /// </summary>
    public DbSet<SmartCardDefinition> SmartCardDefinitions { get; set; }

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
      modelBuilder.ApplyConfiguration(new PracticeSetConfiguration());
      modelBuilder.ApplyConfiguration(new SmartCardConfiguration());
      modelBuilder.ApplyConfiguration(new SmartCardDefinitionConfiguration());
      modelBuilder.ApplyConfiguration(new SmartCardFieldConfiguration());
      modelBuilder.ApplyConfiguration(new SmartCardFieldDefinitionConfiguration());
    }
  }
}