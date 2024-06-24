using System.ComponentModel.DataAnnotations;

namespace SadhanaApp.WebUI.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }

}
