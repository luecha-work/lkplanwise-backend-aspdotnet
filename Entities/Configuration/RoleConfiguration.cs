using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;

namespace Entities.Configuration
{
    public class RoleConfiguration : IEntityTypeConfiguration<Roles>
    {
        public void Configure(EntityTypeBuilder<Roles> builder)
        {
            builder.HasData(
                new Roles
                {
                    Id = Guid.Parse("12345678-1234-1234-1234-123456789012"),
                    RoleCode = "R-001",
                    Name = "Administrator",
                    NormalizedName = "ADMINISTRATOR",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = null,
                    CreatedBy = "Configure",
                    UpdatedBy = null
                },
                new Roles
                {
                    Id = Guid.Parse("22345678-1234-1234-1234-123456789012"),
                    RoleCode = "R-002",
                    Name = "User",
                    NormalizedName = "USER",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = null,
                    CreatedBy = "Configure",
                    UpdatedBy = null
                }
            );
        }
    }
}
