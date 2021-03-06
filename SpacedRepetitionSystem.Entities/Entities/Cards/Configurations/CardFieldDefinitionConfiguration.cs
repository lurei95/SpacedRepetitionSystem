﻿using Microsoft.EntityFrameworkCore;
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
      builder.HasKey(definition => new { definition.CardTemplateId, definition.FieldId });

      builder.Property(definition => definition.FieldName)
        .IsRequired()
        .HasMaxLength(100);
      builder.Property(definition => definition.ShowInputForPractice)
        .IsRequired();
      builder.Property(definition => definition.FieldId)
       .IsRequired();
      builder.Property(definition => definition.CardTemplateId)
        .IsRequired();
      builder.Property(definition => definition.IsRequired)
        .IsRequired();

      builder.HasIndex(field => new { field.CardTemplateId, field.FieldName })
        .IsUnique(); 
    }
  }
}