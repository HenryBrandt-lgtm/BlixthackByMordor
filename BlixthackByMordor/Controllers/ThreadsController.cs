
using BlixthackByMordor.Data;
using BlixthackByMordor.Models;
using BlixthackByMordor.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlixthackByMordor.Controllers
{

    public class ThreadsController(CategoryService categoryService, ThreadService threadService) : Controller

    {
        public IActionResult Index()
        {
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpGet("/Threads/{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            var thread = await threadService.GetThreadById(id);
            if (thread == null)
            {
                return NotFound();
            }

            return View(thread);
        }

        [Authorize]
        public async Task<IActionResult> CreateThread()
        {
            var categories = await categoryService.GetCategories();

            return View(categories);

        }

        [HttpPost]
        public async Task<IActionResult> CreateThread(int categoryId, string title, string content)
        {
            var thread = new ThreadModel
            {
                CategoryId = categoryId,
                Title = title,
                Content = content,
                CreatedAt = DateTime.UtcNow,

                //KOD NEDAN SKALL BORT NÄR VI GJORT USER DELEN
                UserId = 1
            };

            await threadService.CreateThread(thread);

            return RedirectToAction("Index");
        }
    }
}
