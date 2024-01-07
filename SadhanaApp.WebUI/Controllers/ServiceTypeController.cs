using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SadhanaApp.Application.Common.Interfaces;
using SadhanaApp.Domain;
using SadhanaApp.Persistence.Repository;
using System.Security.Claims;

namespace SadhanaApp.WebUI.Controllers
{
    public class ServiceTypeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ServiceTypeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Index Action
        public async Task<IActionResult> Index()
        {
            //var serviceTypes = await _context.ServiceTypes.ToListAsync();
            //return View(serviceTypes);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var serviceTypes = _unitOfWork.ServiceRepository.GetAll(st => st.UserId == int.Parse(userId))
                .ToList();
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
            try
            {
                ModelState.Remove("UserId");
                ModelState.Remove("User");

                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                serviceType.UserId = userId;

                if (ModelState.IsValid)
                {
                    // Check for duplicates within the user's service types
                    bool recordExists = _unitOfWork.ServiceRepository
                        .Any(st => st.ServiceName.Trim().Equals(serviceType.ServiceName.Trim())
                                        && st.UserId == userId);
                    if (recordExists)
                    {
                        TempData["error"] = "This service type already exists.";
                        return View(serviceType);
                    }

                    // Add the new service type
                    _unitOfWork.ServiceRepository.Add(serviceType);
                    _unitOfWork.Save();

                    TempData["success"] = "A new service type has been created successfully.";
                    return RedirectToAction(nameof(Index));
                }

                // If model state is not valid, return the same view to the user for correction
                return View(serviceType);
            }
            catch(Exception e)
            {
                throw;
            }
        }


        // Edit Action (GET)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceType = _unitOfWork.ServiceRepository.Get(s => s.ServiceTypeId ==id);
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

            var existingServiceType = _unitOfWork.ServiceRepository.Get(st => st.ServiceTypeId == id);
            if (existingServiceType == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Preserve the original UserId
                serviceType.UserId = existingServiceType.UserId;

                _unitOfWork.ServiceRepository.Update(serviceType);
                _unitOfWork.Save();

                TempData["success"] = "Service type has been updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            return View(serviceType);
        }


        private bool ServiceTypeExists(int id)
        {
            return _unitOfWork.ServiceRepository.Any(e => e.ServiceTypeId == id);
        }

        // Delete Action (GET)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var serviceType = _unitOfWork.ServiceRepository.Get(m => m.ServiceTypeId == id);

            if (serviceType == null || serviceType.UserId != userId)
            {
                return NotFound(); // Or return unauthorized if you want to indicate a permission issue
            }
            return View(serviceType);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var serviceType =_unitOfWork.ServiceRepository.Get(st => st.ServiceTypeId == id);

            if (serviceType == null || serviceType.UserId != userId)
            {
                return NotFound(); // Or return unauthorized if you want to indicate a permission issue
            }
            _unitOfWork.ServiceRepository.Remove(serviceType);
            _unitOfWork.Save();
            TempData["success"] = "Service type has been deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }

}
