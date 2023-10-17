using System.ComponentModel.DataAnnotations;

namespace SadhanaApp.WebUI.ViewModels
{
    public class UserLoginViewModel
    {
        public string Username { get; set; }

        [DataType(DataType.Password)]
        [MinLength(5, ErrorMessage = "Password must be at least 5 characters long.")]
        public string Password { get; set; }

    }
}
