using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class BlockBruteForce
{
    public Guid BlockforceId { get; set; }

    public string Email { get; set; } = null!;

    public int Count { get; set; }

    /// <summary>
    /// L (Locked): ถูกล็อก
    /// U (UnLock): ไม่ล็อก
    /// </summary>
    public string Status { get; set; } = null!;

    public DateTime? LockedTime { get; set; }

    public DateTime? UnlockTime { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdateAt { get; set; }

    public string? UpdateBy { get; set; }
}
