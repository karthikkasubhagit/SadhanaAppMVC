using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SadhanaApp.Application.Common.Interfaces;
using SadhanaApp.Domain;
using SadhanaApp.Persistence.Repository;
using SadhanaApp.WebUI.ViewModels;
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

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var serviceTypes = _unitOfWork.ServiceRepository
                                .GetAll(st => st.UserId == int.Parse(userId) && !st.IsDeleted)
                                .ToList();

            var activeServiceTypes = serviceTypes.Where(st => !st.IsHidden).ToList();
            var hiddenServiceTypes = serviceTypes.Where(st => st.IsHidden).ToList();

            var viewModel = new ServiceTypeViewModel
            {
                ActiveServiceTypes = activeServiceTypes,
                HiddenServiceTypes = hiddenServiceTypes
            };

            return View(viewModel);
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
                                        && st.UserId == userId && st.IsDeleted == false);
                    if (recordExists)
                    {
                        TempData["error"] = "This service type already exists.";
                        return View(serviceType);
                    }

                    // Add the new service type

                    if (_unitOfWork.ServiceRepository
                        .Any(st => st.ServiceName.Trim().Equals(serviceType.ServiceName.Trim())
                                        && st.UserId == userId))
                    {
                        serviceType.IsDeleted = false;
                        _unitOfWork.ServiceRepository.Add(serviceType);
                    }
                    else
                    {
                        _unitOfWork.ServiceRepository.Add(serviceType);
                    }
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

        // Existing Delete action (GET)
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
                return NotFound();
            }

            bool canDelete = CanDeleteServiceType(serviceType.ServiceName, userId);
            if (!canDelete)
            {
                ViewBag.ErrorMessage = "This service type cannot be deleted as it is in use.";
                return View(serviceType);
            }

            return View(serviceType);
        }

        // Action to hide the service type
        [HttpPost]
        public async Task<IActionResult> HideServiceType(int id)
        {
            var serviceType = _unitOfWork.ServiceRepository.Get(m => m.ServiceTypeId == id);
            if (serviceType == null)
            {
                return NotFound();
            }

            serviceType.IsHidden = true; // Set the flag to hide the service type
            _unitOfWork.ServiceRepository.Update(serviceType);
            _unitOfWork.Save();

            TempData["Message"] = "The service type has been hidden.";
            return RedirectToAction("Index");
        }

        public bool CanDeleteServiceType(string serviceTypeToCheck, int userId)
        {
            // Assuming _dbContext is your database context
            // This query checks if any record for the specified user contains the serviceTypeToCheck
            var chantingRecords = _unitOfWork.SadhanaRepository
                     .GetAll(cr => cr.UserId == userId && cr.ServiceTypeNames != null)
                     .ToList(); // Retrieve data into memory

            bool exists = chantingRecords
                                .Any(cr =>   cr.ServiceTypeNames != null && cr.ServiceTypeNames
                                            .Split(new string[] { ";" }, StringSplitOptions.None)
                                            .Contains(serviceTypeToCheck));


            return !exists; // If it exists, return false (cannot delete), else true (can delete)
        }



        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var serviceType = _unitOfWork.ServiceRepository.Get(st => st.ServiceTypeId == id);

            if (serviceType == null || serviceType.UserId != userId)
            {
                return NotFound(); // Or return unauthorized if you want to indicate a permission issue
            }

            // Soft delete the service type
            serviceType.IsDeleted = true;
            _unitOfWork.ServiceRepository.Update(serviceType);
            _unitOfWork.Save();

            TempData["success"] = "Service type has been deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> UnhideServiceType(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var serviceType = _unitOfWork.ServiceRepository.Get(m => m.ServiceTypeId == id && m.UserId == userId);

            if (serviceType == null)
            {
                return NotFound();
            }

            serviceType.IsHidden = false; // Unhide the service type
            _unitOfWork.ServiceRepository.Update(serviceType);
            _unitOfWork.Save();

            TempData["Message"] = "The service type has been unhidden.";
            return RedirectToAction("Index");
        }

    }

}
