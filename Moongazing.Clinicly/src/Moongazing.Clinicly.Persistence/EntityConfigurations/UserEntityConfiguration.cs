using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Moongazing.Clinicly.Domain.Entities;

namespace Moongazing.Clinicly.Persistence.EntityConfigurations;

public class UserEntityConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(e => e.Id)
                .IsRequired();

        builder.Property(u => u.LastName)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(u => u.Email)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(u => u.PasswordSalt)
               .IsRequired();

        builder.Property(u => u.PasswordHash)
               .IsRequired();

        builder.Property(u => u.Status)
               .IsRequired();

        builder.Property(u => u.AuthenticatorType)
               .IsRequired();



        builder.Property(e => e.CreatedDate)
               .HasColumnType("datetime2")
               .IsRequired(false);

        builder.Property(e => e.DeletedDate)
               .HasColumnType("datetime2")
               .IsRequired(false);

        builder.Property(e => e.UpdatedDate)
               .HasColumnType("datetime2")
               .IsRequired(false);


        builder.HasMany(u => u.UserOperationClaims)
               .WithOne(uoc => uoc.User)
               .HasForeignKey(uoc => uoc.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.RefreshTokens)
               .WithOne(rt => rt.User)
               .HasForeignKey(rt => rt.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.EmailAuthenticators)
               .WithOne(ea => ea.User)
               .HasForeignKey(ea => ea.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.OtpAuthenticators)
               .WithOne(oa => oa.User)
               .HasForeignKey(oa => oa.UserId)
               .OnDelete(DeleteBehavior.Cascade);


        builder.ToTable("Users");
    }
}