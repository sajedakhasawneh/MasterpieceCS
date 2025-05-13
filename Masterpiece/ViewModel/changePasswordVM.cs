using System.ComponentModel.DataAnnotations;
namespace Masterpiece.ViewModel
{
    public class changePasswordVM
    {
 
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The new password must be at least {2} characters.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        [Compare("NewPassword", ErrorMessage = "The confirmation does not match the new password.")]
        public string ConfirmPassword { get; set; }
    }

}

