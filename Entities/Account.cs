using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public partial class Account : IdentityUser<Guid>
    {
        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public bool Active { get; set; }

        public string Title { get; set; }

        public string? Language { get; set; }

        public string? ProfileImageUrl { get; set; }

        public string? ProfileImageName { get; set; }

        public string? CreatedBy { get; set; }

        public string? UpdatedBy { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<AccountRoles> AccountsRole { get; set; } =
            new List<AccountRoles>();

    }
}
