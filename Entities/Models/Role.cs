using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class Role
{
    public Guid Id { get; set; }

    public string RoleCode { get; set; } = null!;

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool Active { get; set; }

    public string? Name { get; set; }

    public string? NormalizedName { get; set; }

    public string? ConcurrencyStamp { get; set; }

    public virtual ICollection<AccountsRole> AccountsRoles { get; set; } = new List<AccountsRole>();

    public virtual ICollection<RoleClaim> RoleClaims { get; set; } = new List<RoleClaim>();
}
