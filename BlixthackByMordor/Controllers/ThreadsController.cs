using Microsoft.AspNetCore.Mvc;

namespace BlixthackByMordor.Controllers
{
    public class ThreadsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
