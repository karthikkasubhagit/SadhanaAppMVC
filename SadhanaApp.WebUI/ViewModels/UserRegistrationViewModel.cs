using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SadhanaApp.WebUI.ViewModels
{
    public class UserRegistrationViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [MinLength(5, ErrorMessage = "Password must be at least 5 characters long.")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Display(Name = "Is ShikshaGuru")]
        public bool IsShikshaGuru { get; set; }

        // Optionally, if we  want the user to select their Shiksha Guru during registration
        [Required(ErrorMessage = "Please select a mentor.")]
        public int? ShikshaGuruId { get; set; }
        public IEnumerable<SelectListItem>? ShikshaGurus { get; set; } // This can be used to populate a dropdown of available ShikshaGurus
    }
}
