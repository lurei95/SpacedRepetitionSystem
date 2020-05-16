﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpacedRepetitionSystem.Entities.Entities.Users;

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
      builder.HasKey(definition => new { definition.UserId, definition.CardTemplateId, definition.FieldName });

      builder.Property(definition => definition.FieldName)
        .IsRequired()
        .HasMaxLength(100);

      builder.Property(definition => definition.UserId)
        .IsRequired();

      builder.Property(definition => definition.ShowInputForPractice)
        .IsRequired();

      builder.Property(definition => definition.CardTemplateId)
        .IsRequired();

      builder.HasOne<User>()
        .WithMany()
        .HasForeignKey(card => card.UserId)
        .OnDelete(DeleteBehavior.NoAction);
    }
  }
}