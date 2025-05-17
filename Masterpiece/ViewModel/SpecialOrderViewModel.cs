using System.ComponentModel.DataAnnotations;
using Masterpiece.Models;
using Microsoft.EntityFrameworkCore;

namespace Masterpiece.ViewModel
{
    public class SpecialOrderViewModel
    {   // From Category table
      
        public List<string> Categories { get; set; } = new();


            // From SpecialOrder table (distinct Occasion values)
            public List<string> Occasions { get; set; } = new List<string>();

        // Form fields
        [Required]
        public string SelectedCategory { get; set; }
            public IFormFile? DesignUpload { get; set; }
            public string? DesignDescription { get; set; }
            public string? ProductSize { get; set; }
            public string? ProductColor { get; set; }
            public string? ProductOccasion { get; set; }
        [Required]
        public DateTime? NeededBy { get; set; }
        [Required]
        public string CustomerName { get; set; }
        [Required]
        public string CustomerEmail { get; set; }
            public string? AdditionalNotes { get; set; }

        [Required]
        public int? MinBudget { get; set; }
        [Required]
        public int? MaxBudget { get; set; }

    }
}
