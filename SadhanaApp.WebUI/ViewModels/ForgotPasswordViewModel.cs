using System.ComponentModel.DataAnnotations;

namespace SadhanaApp.WebUI.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }
    }

}
