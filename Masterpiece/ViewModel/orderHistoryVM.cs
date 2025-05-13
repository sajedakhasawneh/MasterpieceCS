using Masterpiece.Models;

namespace Masterpiece.ViewModel
{
    public class orderHistoryVM
    {
         
    public string ProductName { get; set; }
        public string ImageUrl { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Total => UnitPrice * Quantity;
    }

    public class UserOrderViewModel
    {
        public int OrderId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
        public List<orderHistoryVM> Items { get; set; }

        public User User { get; set; }
    }

}
