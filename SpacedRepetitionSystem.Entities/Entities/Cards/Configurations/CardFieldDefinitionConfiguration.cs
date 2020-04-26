using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SpacedRepetitionSystem.Entities.Entities.Cards.Configurations
{
  /// <summary>
  /// Configuration für <see cref="CardFieldDefinition"/>
  /// </summary>
  public sealed class CardFieldDefinitionConfiguration : IEntityTypeConfiguration<CardFieldDefinition>
  {
    ///<inheritdoc/>
    public void Configure(EntityTypeBuilder<CardFieldDefinition> builder)
    {
      builder.ToTable("CardFieldDefinitions", "Cards");
      builder.HasKey(definition => new { definition.CardTemplateId, definition.FieldName });

      builder.Property(definition => definition.FieldName)
        .IsRequired()
        .HasMaxLength(100);

      builder.Property(definition => definition.CardTemplateId)
        .IsRequired();
    }
  }
}