using System;
using System.Collections.Generic;

namespace Masterpiece.Models;

public partial class SpecialOrder
{
    public int OrderId { get; set; }

    public int UserId { get; set; }

    public string Details { get; set; } = null!;

    public decimal Price { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public string? Img { get; set; }

    public string? Occuaion { get; set; }

    public string? ProductType { get; set; }

    public int? Budget { get; set; }

    public DateOnly? Delivery { get; set; }

    public virtual User User { get; set; } = null!;
}
