using Microsoft.AspNetCore.Mvc;

namespace BlixthackByMordor.Controllers
{
    public class AnswersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
