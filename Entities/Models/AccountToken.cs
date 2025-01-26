using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class AccountToken
{
    public Guid UserId { get; set; }

    public string LoginProvider { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Value { get; set; }

    public virtual Account User { get; set; } = null!;
}
