using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntityConfigurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> entity)
    {
        entity.ToTable("user");

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .HasColumnName("id");

        entity.Property(e => e.Name)
            .HasColumnName("name")
            .HasMaxLength(40)
            .IsRequired();

        entity.Property(e => e.Surname)
            .HasColumnName("surname")
            .HasMaxLength(40)
            .IsRequired();

        entity.Property(e => e.Email)
            .HasColumnName("email")
            .HasMaxLength(256)
            .IsRequired();

        entity.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("ix_user_email");

        entity.Property(e => e.PasswordHash)
            .HasColumnName("password_hash")
            .IsRequired();

        entity.Property(e => e.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .IsRequired();

        entity.Property(u => u.RegistrationTime)
            .HasColumnName("registration_time")
            .IsRequired();

        entity.Property(u => u.LastLoginTime)
            .HasColumnName("last_login_time")
            .IsRequired(false);

        entity.Property(u => u.EmailConfirmationToken)
            .HasColumnName("email_confirmation_token")
            .HasMaxLength(200);

        entity.Property(u => u.EmailConfirmationExpiration)
            .HasColumnName("email_confirmation_expiration")
            .HasColumnType("datetime");
    }
}