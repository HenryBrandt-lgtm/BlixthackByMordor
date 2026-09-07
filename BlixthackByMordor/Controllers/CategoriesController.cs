using Microsoft.AspNetCore.Mvc;

namespace BlixthackByMordor.Controllers
{
    public class CategoriesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
