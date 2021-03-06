﻿using Microsoft.EntityFrameworkCore;
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
      builder.HasKey(template => template.CardTemplateId);

      builder.Property(template => template.CardTemplateId)
        .IsRequired()
        .ValueGeneratedOnAdd();
      builder.Property(template => template.UserId)
        .IsRequired();
      builder.Property(template => template.Title)
        .IsRequired()
        .HasMaxLength(100);

      builder.HasMany(template => template.FieldDefinitions)
        .WithOne(fieldDefinition => fieldDefinition.CardTemplate)
        .HasForeignKey(fieldDefinition => fieldDefinition.CardTemplateId)
        .OnDelete(DeleteBehavior.Cascade);

      builder.HasOne(template => template.User)
        .WithMany()
        .HasForeignKey(template => template.UserId)
        .OnDelete(DeleteBehavior.Cascade);
    }
  }
}