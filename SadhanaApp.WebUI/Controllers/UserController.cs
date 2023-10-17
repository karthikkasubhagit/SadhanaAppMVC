using Microsoft.AspNetCore.Mvc;
using SadhanaApp.WebUI.ViewModels;

namespace SadhanaApp.WebUI.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        //[HttpPost]
        //public async Task<IActionResult> Login(UserLoginViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        // Implement logic to check user credentials and log the user in
        //        // ...

        //        if (/* Credentials are valid */)
        //        {
        //            // Redirect to the home page or another secured area of your app
        //            return RedirectToAction("Index", "Home");
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("", "Invalid login attempt.");
        //        }
        //    }
        //    return View(model);
        //}
    }
}
