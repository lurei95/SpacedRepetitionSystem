using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpacedRepetitionSystem.Entities.Entities.Users;

namespace SpacedRepetitionSystem.Entities.Entities.Cards.Configurations
{
  /// <summary>
  /// Configuration für <see cref="User"/>
  /// </summary>
  public sealed class UserConfiguration : IEntityTypeConfiguration<User>
  {
    ///<inheritdoc/>
    public void Configure(EntityTypeBuilder<User> builder)
    {
      builder.ToTable("User", "User");
      builder.HasKey(user => user.UserId);

      builder.Property(user => user.UserId)
        .IsRequired()
        .ValueGeneratedOnAdd();

      builder.Property(user => user.EmailAddress)
        .IsRequired()
        .HasMaxLength(256);

      builder.Property(user => user.Password)
        .IsRequired();
    }
  }
}