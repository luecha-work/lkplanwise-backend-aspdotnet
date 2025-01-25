using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Configuration
{
    public class RoleConfiguration : IEntityTypeConfiguration<Roles>
    {
        public void Configure(EntityTypeBuilder<Roles> builder)
        {
            builder.HasData(
                new Roles
                {
                    Id = Guid.NewGuid(),
                    Name = "Administrator",
                    NormalizedName = "ADMINISTRATOR",
                    CreatedAt = new DateTime(2023, 1, 1),
                    UpdatedAt = null,
                    CreatedBy = "Configure",
                    UpdatedBy = null
                },
                new Roles
                {
                    Id = Guid.NewGuid(),
                    Name = "User",
                    NormalizedName = "USER",
                    CreatedAt = new DateTime(2023, 1, 1),
                    UpdatedAt = null,
                    CreatedBy = "Configure",
                    UpdatedBy = null
                }
            );
        }
    }
}
