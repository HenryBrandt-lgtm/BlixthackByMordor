using BlixthackByMordor.Models;
using BlixthackByMordor.Services;
using BlixthackByMordor.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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


        [HttpPost]
        [ValidateAntiForgeryToken]
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
                    "An account with this email already exists."
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
                    "Could not create the account."
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
                    "Wrong email or password."
                );

                return View(model);
            }

            await SignInUserAsync(user);

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

        [Authorize]
        [HttpGet("/Users/Profile")]
        public async Task<IActionResult> MyProfile()
        {
            var user = await GetCurrentUserWithActivityAsync();
            if (user == null) return Unauthorized();

            return View("Profile", ProfileViewModel.From(user, isOwner: true));
        }

        [HttpGet("/Users/Profile/{id:int}")]
        public async Task<IActionResult> Profile(int id)
        {
            var user = await _service.GetByIdWithActivityAsync(id);
            if (user == null) return NotFound();

            var isOwner = CurrentUserId() == user.Id;
            return View(ProfileViewModel.From(user, isOwner));
        }

        [Authorize]
        [HttpPost("/Users/Profile")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MyProfile(ProfileViewModel model)
        {
            var user = await GetCurrentUserWithActivityAsync();
            if (user == null) return Unauthorized();

            if (!ModelState.IsValid)
            {
                return RedisplayOwnProfile(model, user);
            }

            if (await _service.EmailTakenByOtherUserAsync(model.Email, user.Id))
            {
                ModelState.AddModelError(
                    nameof(model.Email),
                    "An account with this email already exists."
                );
                return RedisplayOwnProfile(model, user);
            }

            var changingPassword = !string.IsNullOrWhiteSpace(model.NewPassword);
            if (changingPassword && user.Password != model.CurrentPassword)
            {
                ModelState.AddModelError(
                    nameof(model.CurrentPassword),
                    "Current password is incorrect."
                );
                return RedisplayOwnProfile(model, user);
            }

            var updated = await _service.UpdateProfileAsync(
                user.Id,
                model.Username,
                model.Email,
                model.AboutMe,
                changingPassword ? model.NewPassword : null
            );

            if (updated == null)
            {
                ModelState.AddModelError("", "Could not update your profile.");
                return RedisplayOwnProfile(model, user);
            }

            await SignInUserAsync(updated);
            TempData["ProfileSaved"] = true;
            return RedirectToAction(nameof(MyProfile));
        }

        private async Task<UserModel?> GetCurrentUserWithActivityAsync()
        {
            var userId = CurrentUserId();
            if (userId == null) return null;

            return await _service.GetByIdWithActivityAsync(userId.Value);
        }

        private int? CurrentUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return userId == null ? null : int.Parse(userId);
        }

        private IActionResult RedisplayOwnProfile(ProfileViewModel model, UserModel user)
        {
            model.CreatedAt = user.CreatedAt;
            model.IsOwner = true;
            model.AttachActivity(user);
            return View("Profile", model);
        }

        private async Task SignInUserAsync(UserModel user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
            };

            var identity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, principal);
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
