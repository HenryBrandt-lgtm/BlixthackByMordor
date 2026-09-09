using BlixthackByMordor.Models;
using BlixthackByMordor.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlixthackByMordor.Controllers
{
    public class AnswersController(ThreadService threadService, AnswerService answerService) : Controller
    {
        [HttpPost]
        public async Task<IActionResult> Create(int threadId, string content)
        {
            if (string.IsNullOrWhiteSpace(content) || content.Length > 1000)
            {
                return RedirectToAction(nameof(ThreadsController.Details), "Threads", new { id = threadId });
            }

            var thread = await threadService.GetThreadById(threadId);
            if (thread == null)
            {
                return NotFound();
            }

            var answer = new AnswerModel
            {
                ThreadId = threadId,
                Content = content.Trim(),
                CreatedAt = DateTime.UtcNow,
                // Temporary until auth is merged
                UserId = 1
            };

            await answerService.CreateAnswer(answer);

            return RedirectToAction(nameof(ThreadsController.Details), "Threads", new { id = threadId });
        }
    }
}
