using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class AccountLogin
{
    public string LoginProvider { get; set; } = null!;

    public string ProviderKey { get; set; } = null!;

    public string? ProviderDisplayName { get; set; }

    public Guid UserId { get; set; }

    public virtual Account User { get; set; } = null!;
}
