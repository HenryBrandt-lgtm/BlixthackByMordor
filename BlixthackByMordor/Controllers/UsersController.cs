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

        [HttpGet]
        public async Task<IActionResult> Profile(int? id)
        {
            if (id == null)
            {
                if (User.Identity?.IsAuthenticated != true)
                {
                    return Challenge();
                }

                var currentUser = await _service.GetByIdWithActivityAsync(
                    int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!));
                if (currentUser == null) return Unauthorized();

                return View(ToProfileViewModel(currentUser, isOwner: true));
            }

            var user = await _service.GetByIdWithActivityAsync(id.Value);
            if (user == null) return NotFound();

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isOwner = currentUserId != null && currentUserId == user.Id.ToString();

            return View(ToProfileViewModel(user, isOwner));
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return Unauthorized();

            model.CreatedAt = user.CreatedAt;
            model.IsOwner = true;

            if (!ModelState.IsValid)
            {
                await AttachActivityAsync(model, user.Id);
                return View(model);
            }

            if (await _service.EmailTakenByOtherUserAsync(model.Email, user.Id))
            {
                ModelState.AddModelError(
                    nameof(model.Email),
                    "An account with this email already exists."
                );
                await AttachActivityAsync(model, user.Id);
                return View(model);
            }

            var changingPassword = !string.IsNullOrWhiteSpace(model.NewPassword);
            if (changingPassword && user.Password != model.CurrentPassword)
            {
                ModelState.AddModelError(
                    nameof(model.CurrentPassword),
                    "Current password is incorrect."
                );
                await AttachActivityAsync(model, user.Id);
                return View(model);
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
                await AttachActivityAsync(model, user.Id);
                return View(model);
            }

            await SignInUserAsync(updated);
            TempData["ProfileSaved"] = true;
            return RedirectToAction(nameof(Profile));
        }

        private async Task<UserModel?> GetCurrentUserAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return null;

            return await _service.GetByIdAsync(int.Parse(userId));
        }

        private async Task AttachActivityAsync(ProfileViewModel model, int userId)
        {
            var user = await _service.GetByIdWithActivityAsync(userId);
            if (user == null) return;

            model.Threads = ToThreadItems(user);
            model.Answers = ToAnswerItems(user);
        }

        private static ProfileViewModel ToProfileViewModel(UserModel user, bool isOwner)
        {
            return new ProfileViewModel
            {
                Username = user.Username,
                Email = isOwner ? user.Email : string.Empty,
                AboutMe = user.AboutMe,
                CreatedAt = user.CreatedAt,
                IsOwner = isOwner,
                Threads = ToThreadItems(user),
                Answers = ToAnswerItems(user)
            };
        }

        private static IReadOnlyList<ProfileThreadItem> ToThreadItems(UserModel user)
        {
            return (user.Threads ?? [])
                .OrderByDescending(thread => thread.CreatedAt)
                .Select(thread => new ProfileThreadItem
                {
                    Id = thread.Id,
                    Title = thread.Title,
                    CategoryName = thread.Category.Name,
                    CreatedAt = thread.CreatedAt
                })
                .ToList();
        }

        private static IReadOnlyList<ProfileAnswerItem> ToAnswerItems(UserModel user)
        {
            return (user.Answers ?? [])
                .Where(answer => answer.Thread != null)
                .OrderByDescending(answer => answer.CreatedAt)
                .Select(answer => new ProfileAnswerItem
                {
                    ThreadId = answer.ThreadId,
                    ThreadTitle = answer.Thread!.Title,
                    Excerpt = Excerpt(answer.Content),
                    CreatedAt = answer.CreatedAt
                })
                .ToList();
        }

        private static string Excerpt(string content, int maxLength = 140)
        {
            content = content.Trim();
            if (content.Length <= maxLength)
            {
                return content;
            }

            return content[..maxLength].TrimEnd() + "…";
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
