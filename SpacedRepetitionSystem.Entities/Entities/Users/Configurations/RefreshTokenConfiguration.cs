using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpacedRepetitionSystem.Logic.Controllers.Identity;

namespace SpacedRepetitionSystem.Entities.Entities.Users.Configurations
{
  /// <summary>
  /// Configuration für <see cref="RefreshToken"/>
  /// </summary>
  public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
  {
    ///<inheritdoc/>
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
      builder.ToTable("RefreshTokens", "Users");
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