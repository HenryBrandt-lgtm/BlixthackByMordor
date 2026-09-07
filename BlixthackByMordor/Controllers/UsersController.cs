using Microsoft.AspNetCore.Mvc;

namespace BlixthackByMordor.Controllers
{
    public class UsersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
