using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace SadhanaApp.WebUI.ViewModels
{
    public class ProfileViewModel
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }
        public string Email { get; set; }
        public bool IsInstructor { get; set; }
        public string ShikshaGuruName { get; set; }

        public int? ShikshaGuruId { get; set; }

        public IEnumerable<SelectListItem>? ShikshaGurus { get; set; }
       
    }
}