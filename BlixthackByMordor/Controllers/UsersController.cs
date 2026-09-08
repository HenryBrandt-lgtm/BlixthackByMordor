using BlixthackByMordor.Data;
using BlixthackByMordor.Services;
using BlixthackByMordor.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BlixthackByMordor.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserService _service ;
        public UsersController(UserService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }


        public async Task<IActionResult> Register(RegisterViewModel model)
        {
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
            return RedirectToAction("Index", "Home");
        }
        


    }
}
