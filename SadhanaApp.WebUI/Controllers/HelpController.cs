using Microsoft.AspNetCore.Mvc;

namespace SadhanaApp.WebUI.Controllers
{
    public class HelpController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
