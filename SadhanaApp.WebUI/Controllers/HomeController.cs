using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SadhanaApp.Application.Common.Interfaces;
using SadhanaApp.WebUI.Models;
using SadhanaApp.WebUI.ViewModels;
using System.Diagnostics;
using System.Security.Claims;

namespace SadhanaApp.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if(userId == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var id = int.Parse(userId);

                // Get the current date without the time part
                var currentDate = DateTime.Today;

                // Filter records for the last 30 days, last 12 months, and last 5 years

                var records = _unitOfWork.SadhanaRepository.GetAll(c => c.UserId == id && c.Date >= currentDate.AddDays(-30))
                                          .OrderBy(r => r.Date)
                                          .ToList();

                // For daily progress over the last 30 days:

                var dates = records.Select(r => r.Date.ToString("MM-dd-yyyy")).ToList();

                var totalScoresPerDay = records.Select(r => r.TotalScore ?? 0).ToList();

                var viewModel = new GraphViewModel
                {
                    Dates = dates,
                    TotalScoresPerDay = totalScoresPerDay
                };

                return View(viewModel);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in Graph method for user {UserId}.", User.FindFirstValue(ClaimTypes.NameIdentifier));
                return RedirectToAction("Error", "Home");
            }
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