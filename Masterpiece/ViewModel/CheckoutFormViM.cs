namespace Masterpiece.ViewModel
{
    public class CheckoutFormViM
    {
       
            // Customer Info
            public string Name { get; set; }
       
            public string Email { get; set; }

            // Shipping Address
            public string Address { get; set; }
            public string Address2 { get; set; }  // Optional
            public string Country { get; set; }
            public string State { get; set; }
            public string ZipCode { get; set; }

            // Cart Info
            public List<CartItemViewModel> CartItems { get; set; } = new();

            // Payment Info (non-sensitive)
            public string PaymentMethod { get; set; }  // e.g., "CreditCard", "PayPal"
            public decimal Total => CartItems.Sum(item => item.Subtotal); // Computed

            // Optional fields for card input (not stored directly)
            public string? CardHolderName { get; set; }
            public string? PaymentToken { get; set; } // If using gateway (Stripe, etc.)
        }

    
}
