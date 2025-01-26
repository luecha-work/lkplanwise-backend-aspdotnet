using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class Account
{
    public Guid Id { get; set; }

    public string Firstname { get; set; } = null!;

    public string Lastname { get; set; } = null!;

    public bool Active { get; set; }

    public string Title { get; set; } = null!;

    public string? Language { get; set; }

    public string? ProfileImageUrl { get; set; }

    public string? ProfileImageName { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? Username { get; set; }

    public string? Email { get; set; }

    public string? PasswordHash { get; set; }

    public string? SecurityStamp { get; set; }

    public string? ConcurrencyStamp { get; set; }

    public string? Phonenumber { get; set; }

    public DateTimeOffset? LockoutEnd { get; set; }

    public bool LockoutEnabled { get; set; }

    public int AccessFailedCount { get; set; }

    public virtual ICollection<AccountClaim> AccountClaims { get; set; } = new List<AccountClaim>();

    public virtual ICollection<AccountLogin> AccountLogins { get; set; } = new List<AccountLogin>();

    public virtual ICollection<AccountToken> AccountTokens { get; set; } = new List<AccountToken>();

    public virtual ICollection<AccountsRole> AccountsRoles { get; set; } = new List<AccountsRole>();
}
