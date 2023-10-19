using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace SadhanaApp.WebUI.Controllers
{
    public class ChantingController : Controller
    {
        private readonly AppDbContext _context;

        public ChantingController(AppDbContext context)
        {
            _context = context;
        }

        // Display the form to record chanting rounds

        [Authorize]
        public IActionResult RecordChanting()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> RecordChanting(ChantingRecord model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            model.UserId = int.Parse(userId);

            _context.ChantingRecords.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("ChantingHistory");
        }

        // Display the chanting history of the user
        [Authorize]
        public async Task<IActionResult> ChantingHistory()
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

            return View(records ?? new List<ChantingRecord>()); // if records is null, pass an empty list
        }

        // If the user is an instructor, display the chanting history of their students
        [Authorize]
        public async Task<IActionResult> DevoteeChantingHistory()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var devotees = await _context.Users
                .Include(u => u.ChantingRecords)
                .Where(u => u.ShikshaGuruId == int.Parse(userId))
                .ToListAsync();

            return View(devotees);
        }
    }
}
