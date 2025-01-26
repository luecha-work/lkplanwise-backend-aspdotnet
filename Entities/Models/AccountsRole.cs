using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class AccountsRole
{
    public Guid UserId { get; set; }

    public Guid RoleId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public string UpdatedBy { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;

    public virtual Account User { get; set; } = null!;
}
