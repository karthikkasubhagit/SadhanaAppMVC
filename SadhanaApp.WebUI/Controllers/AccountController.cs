using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SadhanaApp.WebUI.Utilities;
using SadhanaApp.WebUI.ViewModels;

namespace SadhanaApp.WebUI.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserRegistrationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userExists = await _context.Users.AnyAsync(u => u.Username == model.Username);
                if (userExists)
                {
                    ModelState.AddModelError("Username", "Username already exists.");
                    return View(model);
                }

                var user = new User
                {
                    Username = model.Username,
                    PasswordHash = PasswordUtility.HashPassword(model.Password),  // Hash the password before storing
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Here you might want to log the user in and redirect to the dashboard, but for simplicity, we'll redirect to the login page.
                return RedirectToAction("Login");
            }

            return View(model);  // Return with validation errors
        }
    }
}
