using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SpacedRepetitionSystem.Entities.Entities.Security.Configurations
{
  /// <summary>
  /// Configuration für <see cref="User"/>
  /// </summary>
  public sealed class UserConfiguration : IEntityTypeConfiguration<User>
  {
    ///<inheritdoc/>
    public void Configure(EntityTypeBuilder<User> builder)
    {
      builder.ToTable("Users", "Security");
      builder.HasKey(user => user.UserId);

      builder.Property(user => user.UserId)
        .IsRequired();
      builder.Property(user => user.Email)
        .IsRequired()
        .HasMaxLength(256);
      builder.Property(user => user.Password)
        .IsRequired();

      builder.Ignore(user => user.AccessToken);
      builder.Ignore(user => user.RefreshToken);
    }
  }
}