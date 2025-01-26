using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class AccountClaim
{
    public int Id { get; set; }

    public Guid UserId { get; set; }

    public string? ClaimType { get; set; }

    public string? ClaimValue { get; set; }

    public virtual Account User { get; set; } = null!;
}
