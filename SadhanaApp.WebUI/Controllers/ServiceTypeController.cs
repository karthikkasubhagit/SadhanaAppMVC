using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SadhanaApp.Domain;
using System.Security.Claims;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace SadhanaApp.WebUI.Controllers
{
    public class ServiceTypeController : Controller
    {
        private readonly AppDbContext _context;

        public ServiceTypeController(AppDbContext context)
        {
            _context = context;
        }

        // Index Action
        public async Task<IActionResult> Index()
        {
            //var serviceTypes = await _context.ServiceTypes.ToListAsync();
            //return View(serviceTypes);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var serviceTypes = await _context.ServiceTypes
                .Where(st => st.UserId == int.Parse(userId))
                .ToListAsync();
            var distinctServiceTypes = serviceTypes
                .GroupBy(st => st.ServiceName)
                .Select(group => group.First())
                .ToList();

            return View(distinctServiceTypes);
        }

        // Create Action (GET)
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceType serviceType)
        {
            ModelState.Remove("UserId");
            ModelState.Remove("User");

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            serviceType.UserId = userId;

            if (ModelState.IsValid)
            {          
                // Check for duplicates within the user's service types
                bool recordExists = await _context.ServiceTypes
                    .AnyAsync(st => st.ServiceName.Trim().Equals(serviceType.ServiceName.Trim())
                                    && st.UserId == userId);
                if (recordExists)
                {
                    TempData["error"] = "This service type already exists.";
                    return View(serviceType);
                }

                // Add the new service type
                _context.ServiceTypes.Add(serviceType);
                await _context.SaveChangesAsync();

                TempData["success"] = "A new service type has been created successfully.";
                return RedirectToAction(nameof(Index));
            }

            // If model state is not valid, return the same view to the user for correction
            return View(serviceType);
        }


        // Edit Action (GET)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceType = await _context.ServiceTypes.FindAsync(id);
            if (serviceType == null)
            {
                return NotFound();
            }
            return View(serviceType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ServiceType serviceType)
        {
            ModelState.Remove("UserId");
            ModelState.Remove("User");
            if (id != serviceType.ServiceTypeId)
            {
                return NotFound();
            }

            var existingServiceType = await _context.ServiceTypes.AsNoTracking().FirstOrDefaultAsync(st => st.ServiceTypeId == id);
            if (existingServiceType == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Preserve the original UserId
                serviceType.UserId = existingServiceType.UserId;

                _context.Update(serviceType);
                await _context.SaveChangesAsync();

                TempData["success"] = "Service type has been updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            return View(serviceType);
        }


        private bool ServiceTypeExists(int id)
        {
            return _context.ServiceTypes.Any(e => e.ServiceTypeId == id);
        }

        // Delete Action (GET)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var serviceType = await _context.ServiceTypes.FirstOrDefaultAsync(m => m.ServiceTypeId == id);

            if (serviceType == null || serviceType.UserId != userId)
            {
                return NotFound(); // Or return unauthorized if you want to indicate a permission issue
            }

            // Check if any chanting records reference this service type
            bool isReferenced = await _context.ChantingRecords.AnyAsync(cr => cr.ServiceTypeId == id);
            if (isReferenced)
            {
                // If referenced, prevent deletion and inform the user
                TempData["error"] = "This service type cannot be deleted because it is referenced by another devotee.";
                return RedirectToAction(nameof(Index));
            }

            return View(serviceType);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var serviceType = await _context.ServiceTypes.FindAsync(id);

            if (serviceType == null || serviceType.UserId != userId)
            {
                return NotFound(); // Or return unauthorized if you want to indicate a permission issue
            }

            // Check if any chanting records reference this service type
            bool isReferenced = await _context.ChantingRecords.AnyAsync(cr => cr.ServiceTypeId == id);
            if (isReferenced)
            {
                // If referenced, prevent deletion and inform the user
                TempData["error"] = "This service type cannot be deleted because it is referenced by other records.";
                return RedirectToAction(nameof(Index));
            }

            _context.ServiceTypes.Remove(serviceType);
            await _context.SaveChangesAsync();
            TempData["success"] = "Service type has been deleted successfully.";
            return RedirectToAction(nameof(Index));
        }



    }

}
