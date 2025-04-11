﻿using System;
using System.Collections.Generic;

namespace Masterpiece.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int OrderId { get; set; }

    public int UserId { get; set; }

    public decimal Amount { get; set; }

    public string Method { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
