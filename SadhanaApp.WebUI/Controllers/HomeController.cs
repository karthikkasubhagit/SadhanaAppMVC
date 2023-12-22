using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SadhanaApp.WebUI.Models;
using System.Diagnostics;

namespace SadhanaApp.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize]
        public IActionResult Sadhana()
        {
            return View();
        }                
        public IActionResult Error()
        {
            return View();
        }
    }
}