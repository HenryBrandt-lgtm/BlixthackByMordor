using BlixthackByMordor.Models;
using BlixthackByMordor.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlixthackByMordor.Controllers
{
    public class ThreadsController(CategoryService categoryService, ThreadService threadService, AnswerService answerService) : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpGet("/Threads/{id:int}")]
        public async Task<IActionResult> Details(int id, int page = 1)
        {
            var thread = await threadService.GetThreadById(id);
            if (thread == null) return NotFound();

            const int pageSize = 10;
            var (answers, totalCount) = await answerService.GetAnswersForThread(id, page, pageSize);

            var replies = new Dictionary<int, List<AnswerModel>>();

            foreach (var answer in answers)
            {
                replies[answer.Id] =
                    await answerService.GetRepliesForAnswer(answer.Id);
            }

            ViewBag.Replies = replies;

            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

           

            ViewBag.Answers = answers;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalAnswers = totalCount;

            return View(thread);
        }
        private bool IsAdmin() => User.IsInRole("Admin");

        [Authorize]
        [HttpGet("/Threads/Create")]
        public async Task<IActionResult> CreateThread()
        {
            var categories = await categoryService.GetCategories();

            return View(categories);
        }

        [Authorize]
        [HttpPost("/Threads/Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateThread(int categoryId, string title, string content)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

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

        [Authorize]
        [HttpPost("/Threads/{id:int}/Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var thread = await threadService.GetThreadById(id);
            if (thread == null) return NotFound();

            var isAdmin = User.IsInRole("Admin");
            if (thread.UserId != int.Parse(userId) && !isAdmin) return Forbid();

            await threadService.DeleteThread(thread);

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [Authorize]
        [HttpGet("/Threads/{id:int}/Edit")]
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsAdmin()) return Forbid();

            var thread = await threadService.GetThreadById(id);
            if (thread == null) return NotFound();

            ViewBag.Categories = await categoryService.GetCategories();

            return View("EditThread", thread);
        }

        [Authorize]
        [HttpPost("/Threads/{id:int}/Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int categoryId, string title)
        {
            if (!IsAdmin()) return Forbid();

            var thread = await threadService.GetThreadById(id);
            if (thread == null) return NotFound();

            if (string.IsNullOrWhiteSpace(title))
            {
                ModelState.AddModelError("title", "Titel får inte vara tom.");
            }

            var categories = await categoryService.GetCategories();
            if (!categories.Any(c => c.Id == categoryId))
            {
                ModelState.AddModelError("categoryId", "Ogiltig kategori.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = categories;
                return View("EditThread", thread);
            }

            thread.Title = title;
            thread.CategoryId = categoryId;

            await threadService.UpdateThread(thread);

            return RedirectToAction("Details", new { id = thread.Id });
        }


        [Authorize]
        [HttpPost("/Threads/{id:int}/Lock")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Lock(int id)

        {
            //ADMIN CHECK
            if (!IsAdmin()) return Forbid();           

            //Hämtar username på den inloggade användaren
            var username = User.FindFirstValue(ClaimTypes.Name);
            if (username == null) return Unauthorized();

            //Hämtar tråden från databasen med det id som kom i URL:en
            var thread = await threadService.GetThreadById(id);
            if (thread == null) return NotFound();

            //Controller skickar till ThreadService. thread = vilken tråd som skall låsas. username = vem som låser den. 
            await threadService.LockThread(thread, username);

            return RedirectToAction(nameof(Details), new { id = thread.Id });
        }

        [Authorize]
        [HttpPost("/Threads/{id:int}/Unlock")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unlock(int id)

        {
            //ADMIN CHECK
            if (!IsAdmin()) return Forbid();          
          

            //Hämtar tråden från databasen med det id som kom i URL:en
            var thread = await threadService.GetThreadById(id);
            if (thread == null) return NotFound();

            //Controller skickar till ThreadService. thread = vilken tråd som skall låsas upp. 
            await threadService.UnlockThread(thread);

            return RedirectToAction(nameof(Details), new { id = thread.Id });
        }


    }
}
