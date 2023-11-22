using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SadhanaApp.Domain;
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
            var serviceTypes = await _context.ServiceTypes.ToListAsync();
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

        // Create Action (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceType serviceType)
        {
            if (ModelState.IsValid)
            {
                bool recordExists = await _context.ServiceTypes
               .AnyAsync(cr => cr.ServiceName.Trim() == serviceType.ServiceName.Trim());
                if (recordExists)
                {
                    TempData["error"] = "This service type already exists.";
                    return View(serviceType);
                }
                _context.ServiceTypes.Add(serviceType);
                await _context.SaveChangesAsync();
                TempData["success"] = "A new service type has been created successfully.";
                return RedirectToAction(nameof(Index));
            }
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

        // Edit Action (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ServiceType serviceType)
        {
            if (id != serviceType.ServiceTypeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(serviceType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServiceTypeExists(serviceType.ServiceTypeId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
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

            var serviceType = await _context.ServiceTypes
                .FirstOrDefaultAsync(m => m.ServiceTypeId == id);
            if (serviceType == null)
            {
                return NotFound();
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

        // Delete Action (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var serviceType = await _context.ServiceTypes.FindAsync(id);
            _context.ServiceTypes.Remove(serviceType);
            await _context.SaveChangesAsync();
            TempData["success"] = "A service type has been deleted successfully.";
            return RedirectToAction(nameof(Index));
        }


    }

}
