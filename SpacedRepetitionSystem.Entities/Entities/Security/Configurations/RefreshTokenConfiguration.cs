using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SpacedRepetitionSystem.Entities.Entities.Security.Configurations
{
  /// <summary>
  /// Configuration für <see cref="RefreshToken"/>
  /// </summary>
  public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
  {
    ///<inheritdoc/>
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
      builder.ToTable("RefreshTokens", "Security");
      builder.HasKey(token => token.TokenId);

      builder.Property(token => token.TokenId).IsRequired();
      builder.Property(token => token.ExpirationDate).IsRequired();
      builder.Property(token => token.Token)
        .IsRequired()
        .HasMaxLength(200)
        .IsUnicode(false);
      builder.Property(token => token.UserId)
        .IsRequired();

      builder.HasOne(token => token.User)
          .WithMany(user => user.RefreshTokens)
          .HasForeignKey(token => token.UserId);
    }
  }
}