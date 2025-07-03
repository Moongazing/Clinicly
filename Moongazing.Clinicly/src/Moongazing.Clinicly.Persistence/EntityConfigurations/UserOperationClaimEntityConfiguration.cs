using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Moongazing.Clinicly.Domain.Entities;

namespace Moongazing.Clinicly.Persistence.EntityConfigurations;

public class UserOperationClaimEntityConfiguration : IEntityTypeConfiguration<UserOperationClaimEntity>
{
    public void Configure(EntityTypeBuilder<UserOperationClaimEntity> builder)
    {
        builder.HasKey(uoc => uoc.Id);

        builder.Property(uoc => uoc.UserId)
               .IsRequired();

        builder.Property(uoc => uoc.OperationClaimId)
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

        builder.HasOne(uoc => uoc.User)
               .WithMany(u => u.UserOperationClaims)
               .HasForeignKey(uoc => uoc.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(uoc => uoc.OperationClaim)
               .WithMany(oc => oc.UserOperationClaims)
               .HasForeignKey(uoc => uoc.OperationClaimId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable("UserOperationClaims");
    }
}