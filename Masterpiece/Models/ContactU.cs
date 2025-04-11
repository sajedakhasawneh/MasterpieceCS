using System;
using System.Collections.Generic;

namespace Masterpiece.Models;

using System.ComponentModel.DataAnnotations;

public partial class ContactU
{
    public int Id { get; set; }

    public int UserId { get; set; }

    [Required(ErrorMessage = "Subject is required")]
    public string Subject { get; set; }

    [Required(ErrorMessage = "Message is required")]
    public string Message { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User? User { get; set; }
}
