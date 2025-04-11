using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace Masterpiece.Models;

public partial class SpecialOrder
{
    public int OrderId { get; set; }

    public int UserId { get; set; }

    [Required(ErrorMessage = "Please provide order details.")]
    public string Details { get; set; }

    [Required(ErrorMessage = "Price is required.")]
    public decimal Price { get; set; }

    public string? Status { get; set; }  // Not submitted via form? Make it nullable

    public DateTime? CreatedAt { get; set; }

    public virtual User? User { get; set; }  // Avoid validation on navigation property
}

//using System.ComponentModel.DataAnnotations;
//using Masterpiece.Models;

//public partial class SpecialOrder
//{
//    public int OrderId { get; set; }

//    public int UserId { get; set; }

//    [Required(ErrorMessage = "Please provide order details.")]
//    public string Details { get; set; }

//    [Required(ErrorMessage = "Price is required.")]
//    public decimal Price { get; set; }

//    public string? Status { get; set; }  // Not submitted via form? Make it nullable

//    public DateTime? CreatedAt { get; set; }

//    public virtual User? User { get; set; }  // Avoid validation on navigation property
//}
