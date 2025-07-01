using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Moongazing.Clinicly.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moongazing.Clinicly.Persistence.EntityConfigurations;

public class OperationClaimEntityConfiguration : IEntityTypeConfiguration<OperationClaimEntity>
{
    public void Configure(EntityTypeBuilder<OperationClaimEntity> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(e => e.Id)
                .IsRequired();

        builder.Property(u => u.Name)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(e => e.CreatedDate)
               .HasColumnType("datetime2")
               .IsRequired(false);

        builder.Property(e => e.DeletedDate)
               .HasColumnType("datetime2")
               .IsRequired(false);

        builder.Property(e => e.UpdatedDate)
               .HasColumnType("datetime2")
               .IsRequired(false);

        builder.HasMany(oc => oc.UserOperationClaims)
               .WithOne(uoc => uoc.OperationClaim)
               .HasForeignKey(uoc => uoc.OperationClaimId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
