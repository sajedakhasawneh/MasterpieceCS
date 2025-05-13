using System.ComponentModel.DataAnnotations;

namespace Masterpiece.ViewModel
{
    public class EditOrderStatusViewModel
    {
        public int OrderId { get; set; }

        [Required]
        public string Status { get; set; } = string.Empty;
    }
}
