using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpacedRepetitionSystem.Logic.Controllers.Identity;

namespace SpacedRepetitionSystem.Entities.Entities.Users.Configurations
{
  public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
  {
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
      builder.ToTable("RefreshTokens", "Users");
      builder.HasKey(token => token.TokenId);

      builder.Property(token => token.TokenId).IsRequired();
      builder.Property(token => token.ExpiryDate).IsRequired();
      builder.Property(token => token.Token)
        .IsRequired()
        .HasMaxLength(200)
        .IsUnicode(false);
      builder.Property(token => token.UserId)
        .IsRequired()
        .HasMaxLength(256);

      builder.HasOne(token => token.User)
          .WithMany(user => user.RefreshTokens)
          .HasForeignKey(token => token.UserId);
    }
  }
}