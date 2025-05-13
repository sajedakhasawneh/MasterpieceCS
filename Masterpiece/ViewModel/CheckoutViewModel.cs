using System.ComponentModel.DataAnnotations;

namespace Masterpiece.ViewModel
{
    public class CheckoutViewModel
    {
        // User info
        [Required] public string Name { get; set; }
        [Required][EmailAddress] public string Email { get; set; }
        [Required] public string Address { get; set; }

        // Payment
        [Required] public string PaymentMethod { get; set; }

        // Voucher
        public string VoucherCode { get; set; }
        public decimal DiscountAmount { get; set; }

        // Cart
        public List<CartItemViewModel> CartItems { get; set; } = new();
        public decimal TotalBeforeDiscount => CartItems.Sum(i => i.Price * i.Quantity);
        public decimal TotalAfterDiscount => TotalBeforeDiscount - DiscountAmount;
    }
}
