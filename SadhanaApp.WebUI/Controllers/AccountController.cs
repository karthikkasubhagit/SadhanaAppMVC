using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SadhanaApp.WebUI.Utilities;
using SadhanaApp.WebUI.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using System.Linq.Expressions;
using SadhanaApp.Application.Common.Interfaces;

namespace SadhanaApp.WebUI.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IUnitOfWork unitOfWork, ILogger<AccountController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }


        [HttpGet]
        public IActionResult Register()
        {
            try
            {
                var shikshaGurus = _unitOfWork.UserRepository.GetAll(u => u.IsInstructor) // Assuming IsInstructor is equivalent property for IsShikshaGuru
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in Register Get method for user {UserId}.", User.FindFirstValue(ClaimTypes.NameIdentifier));
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserRegistrationViewModel model)
        {
            try
            {
                var shikshaGurus = _unitOfWork.UserRepository.GetAll(u => u.IsInstructor)
                             .Select(sg => new SelectListItem
                             {
                                 Value = sg.UserId.ToString(),
                                 Text = $"{sg.FirstName} {sg.LastName}"
                             }).ToList();
                model.ShikshaGurus = shikshaGurus;

                if (ModelState.IsValid)
                {
                    var userExists = _unitOfWork.UserRepository.Get(u => u.Username == model.Username);
                    if (userExists != null)
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

                    _unitOfWork.UserRepository.Add(user);
                    _unitOfWork.UserRepository.Save();

                    // Here you might want to log the user in and redirect to the dashboard, but for simplicity, we'll redirect to the login page.
                    return RedirectToAction("Login");
                }

                return View(model);  // Return with validation errors
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in Register Post method for user {UserId}.", User.FindFirstValue(ClaimTypes.NameIdentifier));
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in Login Get method for user {UserId}.", User.FindFirstValue(ClaimTypes.NameIdentifier));
                return RedirectToAction("Error", "Home");
            }
        }

        public async Task GoogleLogin()
        {
            try
            {
                await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme,
                                            new AuthenticationProperties()
                                            {
                                                RedirectUri = Url.Action("GoogleResponse")
                                            });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in GoogleLogin method for user {UserId}.", User.FindFirstValue(ClaimTypes.NameIdentifier));
            }
        }

        public async Task<IActionResult> GoogleResponse()
        {
            try
            {
                var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                //var claims = result.Principal.Identities.FirstOrDefault()?.Claims.Select(claim => new
                //{
                //    claim.Issuer,
                //    claim.OriginalIssuer,
                //    claim.Type,
                //    claim.Value
                //});

                var claims = result.Principal.Identities.FirstOrDefault()?.Claims.ToList();

                var emailClaim = claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
                var firstNameClaim = claims?.FirstOrDefault(x => x.Type == ClaimTypes.GivenName)?.Value;
                var lastNameClaim = claims?.FirstOrDefault(x => x.Type == ClaimTypes.Surname)?.Value;


                if (string.IsNullOrEmpty(emailClaim))
                {
                    // Handle error: Email is essential
                    return RedirectToAction("Login");
                }

                var user = _unitOfWork.UserRepository.Get(u => u.Email == emailClaim);

                if (user != null)
                {
                    // User exists, sign in
                    var claimSavedDetails = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Username),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString())
                    };

                    var roleInfor = user.IsInstructor ? "Instructor" : "User";  // Assuming "User" as default role
                    claimSavedDetails.Add(new Claim(ClaimTypes.Role, roleInfor));

                    var claimsIdentityNewUser = new ClaimsIdentity(claimSavedDetails, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentityNewUser));

                    return RedirectToAction("Index", "Home");
                }

                if (user == null)
                {
                    // User does not exist, create a new user with minimal data
                    user = new User
                    {
                        Username = firstNameClaim,
                        PasswordHash = "External",  // Hash the token before storing
                        Email = emailClaim,
                        FirstName = firstNameClaim,
                        LastName = lastNameClaim,
                        DateRegistered = DateTime.UtcNow,
                        IsInstructor = false,
                    };

                    _unitOfWork.UserRepository.Add(user);
                    _unitOfWork.UserRepository.Save();
                }

                var claimDetails = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Username),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString())
                    };

                var role = user.IsInstructor ? "Instructor" : "User";  // Assuming "User" as default role
                claimDetails.Add(new Claim(ClaimTypes.Role, role));

                var claimsIdentity = new ClaimsIdentity(claimDetails, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                return RedirectToAction("ProfileCompletion", new { userId = user.UserId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in GoogleResponse method for user {UserId}.", User.FindFirstValue(ClaimTypes.NameIdentifier));
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ProfileCompletion()
        {
            try
            {
                // Assuming the user is already signed in at this point
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Load ShikshaGurus for dropdown
                var shikshaGurus = _unitOfWork.UserRepository.GetAll(u => u.IsInstructor)
                    .Select(sg => new SelectListItem
                    {
                        Value = sg.UserId.ToString(),
                        Text = $"{sg.FirstName} {sg.LastName}"
                    });

                var viewModel = new ProfileCompletionViewModel
                {
                    ShikshaGurus = shikshaGurus.ToList()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in ProfileCompletion Get method for user {UserId}.", User.FindFirstValue(ClaimTypes.NameIdentifier));
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ProfileCompletion(ProfileCompletionViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Retrieve the currently logged-in user's ID
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (userId == null)
                    {
                        // Handle the case where the user ID is not found
                        return RedirectToAction("Error", "Home");
                    }

                    var user = _unitOfWork.UserRepository.Get(u => u.UserId == int.Parse(userId));
                    if (user == null)
                    {
                        // Handle the case where the user is not found in the database
                        return RedirectToAction("Error", "Home");
                    }

                    // Update the user's profile with the information from the form
                    user.IsInstructor = model.IsShikshaGuru;
                    user.ShikshaGuruId = model.ShikshaGuruId;

                    // Save changes to the database
                    _unitOfWork.UserRepository.Update(user);
                    _unitOfWork.UserRepository.Save();

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



                // In case of any validation errors, return the same view for correction
                // Reload ShikshaGurus to repopulate the dropdown
                model.ShikshaGurus = _unitOfWork.UserRepository.GetAll(u => u.IsInstructor)
                                    .Select(sg => new SelectListItem
                                    {
                                        Value = sg.UserId.ToString(),
                                        Text = $"{sg.FirstName} {sg.LastName}"
                                    }).ToList();

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in ProfileCompletion Get method for user {UserId}.", User.FindFirstValue(ClaimTypes.NameIdentifier));
                return RedirectToAction("Error", "Home");
            }
        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = _unitOfWork.UserRepository.Get(u => u.Username == model.Username); // Note: Avoid storing plain text passwords

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

                        return RedirectToAction("Index", "Home");
                    }

                    ModelState.AddModelError("", "Invalid login attempt, Please try again.");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in Login Post method for user {UserId}.", User.FindFirstValue(ClaimTypes.NameIdentifier));
                return RedirectToAction("Error", "Home");
            }
        }

        // Logout functionality
        public async Task<IActionResult> Logout()
        {
            try
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in Logout method for user {UserId}.", User.FindFirstValue(ClaimTypes.NameIdentifier));
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in ForgotPassword Get method for user {UserId}.", User.FindFirstValue(ClaimTypes.NameIdentifier));
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = _unitOfWork.UserRepository.Get(u => u.Username == model.Username);
                    if (user != null)
                    {
                        // Redirect to the reset password page
                        return RedirectToAction("ResetPassword", new { userId = user.UserId });
                    }
                    ModelState.AddModelError("", "Username not found.");
                }
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in ForgotPassword Post method for user {UserId}.", User.FindFirstValue(ClaimTypes.NameIdentifier));
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        public IActionResult ResetPassword(int userId)
        {
            try
            {
                var model = new ResetPasswordViewModel { UserId = userId };
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in ResetPassword Get method for user {UserId}.", User.FindFirstValue(ClaimTypes.NameIdentifier));
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = _unitOfWork.UserRepository.Get(u => u.UserId == model.UserId);
                    if (user != null)
                    {
                        // Update the user's password
                        user.PasswordHash = PasswordUtility.HashPassword(model.NewPassword);
                        _unitOfWork.UserRepository.Update(user);
                        _unitOfWork.UserRepository.Save();

                        return RedirectToAction("Login");
                    }
                    ModelState.AddModelError("", "User not found.");
                }
                return View(model);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred in ResetPassword Post method for user {UserId}.", User.FindFirstValue(ClaimTypes.NameIdentifier));
                return RedirectToAction("Error", "Home");            
            }
        }
    }
}
