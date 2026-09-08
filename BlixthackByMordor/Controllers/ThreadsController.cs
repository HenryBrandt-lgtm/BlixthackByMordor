using BlixthackByMordor.Data;
using BlixthackByMordor.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlixthackByMordor.Controllers
{
    public class ThreadsController(ApplicationDbContext db) : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpGet("/Threads/{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            var thread = await new ThreadService(db).GetThreadById(id);
            if (thread == null)
            {
                return NotFound();
            }

            return View(thread);
        }
    }
}
