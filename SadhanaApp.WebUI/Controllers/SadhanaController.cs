using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SadhanaApp.WebUI.ViewModels;
using System.Security.Claims;

namespace SadhanaApp.WebUI.Controllers
{
    public class SadhanaController : Controller
    {
        private readonly AppDbContext _context;

        public SadhanaController(AppDbContext context)
        {
            _context = context;
        }

        // Display the form to record chanting rounds

        [Authorize]
        public IActionResult RecordSadhana()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> RecordSadhana(ChantingRecord model)
        {
            // if (!ModelState.IsValid)  // Need to fix this issue
            // {
            //     return View(model); // Return the same view with validation messages
            // }

            // if the ServiceType is "other", then update it with the custom service type provided

                if (model.ServiceType == "other")
                {
                    model.ServiceType = Request.Form["customServiceTypeInput"];
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                model.UserId = int.Parse(userId);

                _context.ChantingRecords.Add(model);
                await _context.SaveChangesAsync();

                return RedirectToAction("SadhanaHistory");
           
        }

        // Display the chanting history of the user
        [Authorize]
        public async Task<IActionResult> SadhanaHistory()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // Ensure userId is not null before proceeding
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var records = await _context.ChantingRecords
                .Where(c => c.UserId == int.Parse(userId))
                .ToListAsync();

            // Define the date range, e.g., for the past 30 days
            var startDate = DateTime.Today.AddDays(-30);
            var endDate = DateTime.Today;

            // Generate a list of dates for this range
            var allDates = Enumerable.Range(0, (endDate - startDate).Days + 1)
                            .Select(d => startDate.AddDays(d))
                            .ToList();

            // Find missing dates
            var missingDates = allDates.Except(records.Select(r => r.Date)).ToList();

            // Create a ViewModel (or use a Tuple) to pass both records and missing dates to the view
            var model = new Tuple<List<ChantingRecord>, List<DateTime>>(records, missingDates);

            return View(model);
        }

        // If the user is an instructor, display the chanting history of their students
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> DevoteeSadhanaHistory()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var devotees = await _context.Users
                .Include(u => u.ChantingRecords)
                .Where(u => u.ShikshaGuruId == int.Parse(userId))
                .ToListAsync();

            // Convert the list of devotees into a SelectList
            ViewBag.DevoteeList = new SelectList(devotees, "UserId", "FirstName");

            return View(devotees);
        }

        [Authorize]

        public IActionResult Graph()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var id = int.Parse(userId);
            var records = _context.ChantingRecords.Where(u => u.UserId == id).OrderBy(r => r.Date).ToList();

            // For daily progress:
            var dates = records.Select(r => r.Date.ToString("MM-dd-yyyy")).ToList();
            var totalScoresPerDay = records.Select(r => r.TotalScore ?? 0).ToList();

            // For monthly progress:
            var monthlyData = records.GroupBy(r => new { r.Date.Year, r.Date.Month })
                                     .Select(g => new
                                     {
                                         Month = $"{g.Key.Month}-{g.Key.Year}",
                                         TotalScore = g.Sum(x => x.TotalScore) ?? 0
                                     }).ToList();

            var months = monthlyData.Select(m => m.Month).ToList();
            var totalScoresPerMonth = monthlyData.Select(m => m.TotalScore).ToList();

            // For yearly progress:
            var yearlyData = records.GroupBy(r => r.Date.Year)
                                    .Select(g => new
                                    {
                                        Year = g.Key.ToString(),
                                        TotalScore = g.Sum(x => x.TotalScore) ?? 0
                                    }).ToList();

            var years = yearlyData.Select(y => y.Year).ToList();
            var totalScoresPerYear = yearlyData.Select(y => y.TotalScore).ToList();

            var viewModel = new GraphViewModel
            {
                Dates = dates,
                TotalScoresPerDay = totalScoresPerDay,
                Months = months,
                TotalScoresPerMonth = totalScoresPerMonth,
                Years = years,
                TotalScoresPerYear = totalScoresPerYear
            };

            return View(viewModel);
        }

        // Display the Edit form
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var record = await _context.ChantingRecords.FindAsync(id);
            if (record == null)
            {
                return NotFound();
            }

            return View(record);
        }

        // Handle the Edit form submission
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(int id, ChantingRecord model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            // if (!ModelState.IsValid)
            // {
            //     return View(model); // Return the same view with validation messages
            // }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Ensure that the user is modifying their own record
            if (model.UserId != int.Parse(userId))
            {
                return Unauthorized();
            }

            if (model.ServiceType == "Other")
            {
                model.ServiceType = Request.Form["customServiceTypeInput"];
            }

            _context.Entry(model).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Handle the exception (e.g., record doesn't exist anymore)
                return NotFound();
            }
            return RedirectToAction("SadhanaHistory");
        }

    }
}
