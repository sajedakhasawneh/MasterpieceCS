using Masterpiece.Models;

namespace Masterpiece.ViewModel
{
    //public class CartsVM
    //{
    //    //public int ProductId { get; set; }
    //    //public int Quantity { get; set; }

    //    //public int UserId { get; set; }

    //    //public string Size { get; set; }

    //    //public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    //    //public virtual ICollection<Product> Products { get; set; } = new List<Product>();


    //}

    public class CartItemViewModel
    {
        public int CartItemId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        // Calculated property for subtotal
        public decimal Subtotal => Price * Quantity;
    }

    public class CartsVM
    {
        public ICollection<CartItemViewModel> CartItems { get; set; } = new List<CartItemViewModel>();
    }
}
