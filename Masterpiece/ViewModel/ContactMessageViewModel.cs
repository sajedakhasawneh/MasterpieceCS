namespace Masterpiece.ViewModel
{
    public class ContactMessageViewModel
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }

        // Additional data from User
        public string? UserName { get; set; }
    }


}
