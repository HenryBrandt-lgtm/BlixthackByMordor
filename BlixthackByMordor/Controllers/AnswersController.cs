using BlixthackByMordor.Models;
using BlixthackByMordor.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlixthackByMordor.Controllers
{
    public class AnswersController(ThreadService threadService, AnswerService answerService) : Controller
    {
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int threadId, string content)
        {
            if (string.IsNullOrWhiteSpace(content) || content.Length > 1000)
            {
                return RedirectToAction(nameof(ThreadsController.Details), "Threads", new { id = threadId });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var thread = await threadService.GetThreadById(threadId);
            if (thread == null)
            {
                return NotFound();
            }
            if (thread.ThreadLocked)
            {
                return Forbid();
            }

            var answer = new AnswerModel
            {
                ThreadId = threadId,
                Content = content.Trim(),
                CreatedAt = DateTime.UtcNow,
                UserId = int.Parse(userId)
            };

            await answerService.CreateAnswer(answer);

            return RedirectToAction(nameof(ThreadsController.Details), "Threads", new { id = threadId });
        }
        [Authorize]
        [HttpPost("/Answers/{id:int}/Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var answer = await answerService.GetAnswerById(id);
            if (answer == null) return NotFound();

            var isAdmin = User.IsInRole("Admin");
            if (answer.UserId != int.Parse(userId) && !isAdmin) return Forbid();

            await answerService.DeleteAnswer(answer);

            return RedirectToAction(nameof(ThreadsController.Details), "Threads", new { id = answer.ThreadId });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateReply(int threadId, string content, int answerId)
        {
            if (string.IsNullOrWhiteSpace(content) || content.Length > 1000)
            {
                return RedirectToAction(nameof(ThreadsController.Details), "Threads", new { id = threadId });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var thread = await threadService.GetThreadById(threadId);
            if (thread == null)
            {
                return NotFound();
            }
            if (thread.ThreadLocked)
            {
                return Forbid();
            }

            var answer = new AnswerModel
            {
                ThreadId = threadId,
                Content = content.Trim(),
                CreatedAt = DateTime.UtcNow,
                UserId = int.Parse(userId),
                ReplyId = answerId,
            };

            await answerService.CreateAnswer(answer);

            return RedirectToAction(nameof(ThreadsController.Details), "Threads", new { id = threadId });
        }



    }
}
