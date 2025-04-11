using System;
using System.Collections.Generic;

namespace Masterpiece.Models;

public partial class Shipping
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public string Address { get; set; } = null!;

    public string SecondAddress { get; set; } = null!;

    public string City { get; set; } = null!;

    public string Country { get; set; } = null!;

    public string PostalCode { get; set; } = null!;

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Order Order { get; set; } = null!;
}
