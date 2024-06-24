using System.ComponentModel.DataAnnotations;

namespace SadhanaApp.WebUI.ViewModels
{
    public class ResetPasswordViewModel
    {
        public int UserId { get; set; }

        public string? UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(5, ErrorMessage = "Password must be at least 5 characters long.")]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

}
