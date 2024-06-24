using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using NuGet.Protocol;
using SadhanaApp.Application.Common.Interfaces;
using SadhanaApp.Persistence.Repository;
using Microsoft.EntityFrameworkCore;
using SadhanaApp.WebUI.ViewModels;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace YourApp.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProfileController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        [HttpGet]
        public IActionResult Index()
        {
            
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return NotFound("User not logged in.");  // or redirect to a login page
            }

            int userId = int.Parse(userIdClaim);
            var user = _unitOfWork.UserRepository.Get(
                x => x.UserId == userId,
                includeProperties: "ShikshaGuru"  // Assuming you need related data
            );

            if (user == null)
            {
                return NotFound("User not found.");
            }

            var model = new ProfileViewModel
            {
                UserId = userId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                IsInstructor = user.IsInstructor ,
                ShikshaGuruName = user.ShikshaGuru?.Username ?? "You do not have a mentor."
            };

            ViewBag.Instructor = user.IsInstructor ? "Yes" : "No";
            return View(model);
        }


        public IActionResult Edit(int id)
        {
            var user = _unitOfWork.UserRepository.Get(
                x => x.UserId == id,
                includeProperties: "ShikshaGuru"  // Assuming you need related data
            );

            var shikshaGurus = _unitOfWork.UserRepository.GetAll(u => u.IsInstructor)
                            .Select(sg => new SelectListItem
                            {
                                Value = sg.UserId.ToString(),
                                Text = $"{sg.FirstName} {sg.LastName}"
                            }).ToList();

            if (user == null)
            {
                return NotFound("User not found.");
            }

            var model = new ProfileViewModel
            {
                UserId = id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                IsInstructor = user.IsInstructor 
            };
            model.ShikshaGurus = shikshaGurus;

            return View(model);
        }



        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProfileViewModel model)
        {

            // Get the existing user from the database
            var user = _unitOfWork.UserRepository.Get(u => u.UserId == model.UserId, includeProperties: "ShikshaGuru");
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Update the user properties from the model
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.IsInstructor = model.IsInstructor;

            if(model.ShikshaGuruId != null)
            {
                user.ShikshaGuruId = model.ShikshaGuruId;
            }

            try
            {
                _unitOfWork.UserRepository.Update(user);
                _unitOfWork.Save();  // Assuming SaveAsync() exists and saves all changes
                return RedirectToAction(nameof(Index)); // Redirect to the Index or any other appropriate view
            }
            catch (Exception ex)
            {
                {
                    // Handle exceptions, possibly logging them and informing the user
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                    return View(model);
                }
            }

        }
        }
    }
