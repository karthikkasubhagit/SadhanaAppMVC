using Microsoft.AspNetCore.Mvc.Rendering;

namespace SadhanaApp.WebUI.ViewModels
{
    public class InstructorStudentGraphViewModel
    {
        public List<SelectListItem> Students { get; set; } = new List<SelectListItem>();
        // You can add more properties if needed to hold graph data
    }

}
