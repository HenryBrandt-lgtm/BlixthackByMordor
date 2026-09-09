using BlixthackByMordor.Data;
using BlixthackByMordor.Models;
using BlixthackByMordor.Services;
using BlixthackByMordor.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BlixthackByMordor.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserService _service;
        public UsersController(UserService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult Register(string? returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }


        public async Task<IActionResult> Register(RegisterViewModel model, string? returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }


            if (await _service.EmailExistsAsync(model.Email))
            {
                ModelState.AddModelError(
                    "Email",
                    "Det finns redan ett konto med denna e-postadress."
                );

                return View(model);
            }
            var user =
           await _service.RegisterAsync(
               model.Username,
               model.Email,
               model.Password
           );

            if (user == null)
            {
                ModelState.AddModelError(
                    "",
                    "Kunde inte skapa kontot."
                );

                return View(model);
            }
            //Add a succed view
            return RedirectToAction(nameof(Login), new { returnUrl });
        }


        [HttpGet]
        public IActionResult Login(string? returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(
        LoginViewModel model, string? returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _service.LoginAsync(model.Email,model.Password);


            if (user == null)
            {
                ModelState.AddModelError(
                    "",
                    "Fel e-postadress eller lösenord."
                );

                return View(model);
            }

            var claims = new List<Claim>()
            {
                new Claim( ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Name,user.Username),
                new Claim(ClaimTypes.Email,user.Email),
            };

            var identity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var princpal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, princpal);

            return RedirectAfterLogin(returnUrl);


        }


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults
                    .AuthenticationScheme
            );

            return RedirectToAction("Index","Home");
        }


        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        private IActionResult RedirectAfterLogin(string? returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl)
                && Url.IsLocalUrl(returnUrl)
                && !returnUrl.StartsWith("/Answers", StringComparison.OrdinalIgnoreCase))
            {
                return LocalRedirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }


    }
}
