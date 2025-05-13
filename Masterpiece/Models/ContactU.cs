using System;
using System.Collections.Generic;

namespace Masterpiece.Models;

public partial class ContactU
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string Subject { get; set; } = null!;

    public string Message { get; set; } = null!;

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User? User { get; set; }
}
