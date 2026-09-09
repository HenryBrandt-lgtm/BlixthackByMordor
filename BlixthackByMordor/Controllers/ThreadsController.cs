using BlixthackByMordor.Models;
using BlixthackByMordor.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var thread = new ThreadModel
            {
                CategoryId = categoryId,
                Title = title,
                Content = content,
                CreatedAt = DateTime.UtcNow,
                UserId = int.Parse(userId)
            };

            await threadService.CreateThread(thread);

            return RedirectToAction("Index");
        }
    }
}
