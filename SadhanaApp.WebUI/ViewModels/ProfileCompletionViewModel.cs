using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SadhanaApp.WebUI.ViewModels
{
    public class ProfileCompletionViewModel
    {
        [Display(Name = "Do you mentor others?")]
        public bool IsShikshaGuru { get; set; }

        [Required(ErrorMessage = "Please select your mentor from the list")]
        [Display(Name = "Select your mentor")]
        public int? ShikshaGuruId { get; set; }

        public List<SelectListItem> ShikshaGurus { get; set; } = new List<SelectListItem>();
    }
}
