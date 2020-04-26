using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SpacedRepetitionSystem.Entities.Entities.Cards.Configurations
{
  /// <summary>
  /// Configuration für <see cref="CardTemplate"/>
  /// </summary>
  public sealed class CardTemplateConfiguration : IEntityTypeConfiguration<CardTemplate>
  {
    ///<inheritdoc/>
    public void Configure(EntityTypeBuilder<CardTemplate> builder)
    {
      builder.ToTable("CardTemplates", "Cards");
      builder.HasKey(definition => definition.CardTemplateId);

      builder.Property(definition => definition.CardTemplateId)
        .IsRequired()
        .ValueGeneratedOnAdd();

      builder.Property(definition => definition.Title)
        .IsRequired()
        .HasMaxLength(100);

      builder.HasMany(cardDefinition => cardDefinition.FieldDefinitions)
        .WithOne(fieldDefinition => fieldDefinition.CardDefinition)
        .HasForeignKey(fieldDefinition => fieldDefinition.CardTemplateId)
        .OnDelete(DeleteBehavior.Cascade);
    }
  }
}