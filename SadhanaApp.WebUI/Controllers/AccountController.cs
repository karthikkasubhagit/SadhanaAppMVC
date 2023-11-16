using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SadhanaApp.WebUI.Utilities;
using SadhanaApp.WebUI.ViewModels;
using System.Security.Claims;

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
            //var viewModel = new UserRegistrationViewModel
            //{
            //    ShikshaGurus = new List<SelectListItem>
            //    {
            //        new SelectListItem { Value = "1", Text = "Parijanya Das" },
            //        new SelectListItem { Value = "2", Text = "Ambarish Das" },
            //        new SelectListItem { Value = "3", Text = "Sanaka Santan Das" }
            //    }
            //};

            var shikshaGurus = _context.Users
                               .Where(u => u.IsInstructor) // Assuming IsInstructor is equivalent property for IsShikshaGuru
                               .ToList();

            var viewModel = new UserRegistrationViewModel
            {
                ShikshaGurus = shikshaGurus.Select(sg => new SelectListItem
                {
                    Value = sg.UserId.ToString(),
                    Text = $"{sg.FirstName} {sg.LastName}"  // You can format this as per your preference
                })
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserRegistrationViewModel model)
        {
            var shikshaGurus = _context.Users
                         .Where(u => u.IsInstructor)
                         .Select(sg => new SelectListItem
                         {
                             Value = sg.UserId.ToString(),
                             Text = $"{sg.FirstName} {sg.LastName}"
                         }).ToList();
            model.ShikshaGurus = shikshaGurus;

            if (ModelState.IsValid)
            {
                var userExists = await _context.Users.AnyAsync(u => u.Username == model.Username);
                if (userExists)
                {
                    ModelState.AddModelError("Username", "Username already exists.");
                    return View(model);
                }

                User user;
                if (model.IsShikshaGuru)
                {
                    user = new User
                    {
                        Username = model.Username,
                        PasswordHash = PasswordUtility.HashPassword(model.Password),  // Hash the password before storing
                        Email = model.Email,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        IsInstructor = model.IsShikshaGuru
                    };
                }
                else
                {
                    user = new User
                    {
                        Username = model.Username,
                        PasswordHash = PasswordUtility.HashPassword(model.Password),  // Hash the password before storing
                        Email = model.Email,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        ShikshaGuruId = model.ShikshaGuruId
                    };
                    // Assign the user to the selected ShikshaGuru
                    // using model.ShikshaGuruId
                }

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Here you might want to log the user in and redirect to the dashboard, but for simplicity, we'll redirect to the login page.
                return RedirectToAction("Login");
            }

            return View(model);  // Return with validation errors
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.FirstOrDefault(u => u.Username == model.Username); // Note: Avoid storing plain text passwords

                if (user != null && PasswordUtility.HashPassword(model.Password) == user.PasswordHash)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Username),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString())
                    };

                    var role = user.IsInstructor ? "Instructor" : "User";  // Assuming "User" as default role
                    claims.Add(new Claim(ClaimTypes.Role, role));

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                    return RedirectToAction("SadhanaHistory", "Sadhana");
                }

                ModelState.AddModelError("", "Invalid login attempt, Please try again.");
            }

            return View(model);
        }

        // Logout functionality
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username);
                if (user != null)
                {
                    // Redirect to the reset password page
                    return RedirectToAction("ResetPassword", new { userId = user.UserId });
                }
                ModelState.AddModelError("", "Username not found.");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult ResetPassword(int userId)
        {
            var model = new ResetPasswordViewModel { UserId = userId };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users.FindAsync(model.UserId);
                if (user != null)
                {
                    // Update the user's password
                    user.PasswordHash = PasswordUtility.HashPassword(model.NewPassword);
                    _context.Update(user);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Login");
                }
                ModelState.AddModelError("", "User not found.");
            }
            return View(model);
        }



    }
}
