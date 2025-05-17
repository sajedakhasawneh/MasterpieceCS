using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Masterpiece.Models;

public partial class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int Stock { get; set; }

    public int CategoryId { get; set; }

    public int OwnerId { get; set; }

    public string? ImageUrl { get; set; }

    public int? Rating { get; set; }

    public string? Color { get; set; }

    public string? Size { get; set; }

    public string? Img1 { get; set; }

    public string? Img2 { get; set; }

    public string? Img3 { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    [ValidateNever]
    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    [ValidateNever]
    public virtual User Owner { get; set; } = null!;

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}
