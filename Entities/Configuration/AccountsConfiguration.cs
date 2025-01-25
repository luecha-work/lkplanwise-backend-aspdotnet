using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Configuration
{
    public class AccountsConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.HasData(
                new Account
                {
                    Id = Guid.NewGuid(),
                    Firstname = "John",
                    Lastname = "Doe",
                    Active = true,
                    Title = "Mr.",
                    Language = "English",
                    ProfileImageUrl = null,
                    ProfileImageName = null,
                    CreatedBy = "Configure",
                    UpdatedBy = null,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = null
                }
            );
        }
    }
}
