using BlixthackByMordor.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlixthackByMordor.Controllers
{
    public class UserFavoriteController : Controller
    {

        private readonly FavoriteAnswerService _service;
        public UserFavoriteController(FavoriteAnswerService service)
        {
            _service = service;
        }



        public IActionResult Index()
        {
            return View();
        }

        [Authorize]

        public async Task<IActionResult> ToggleFavorite(int answerId)
        {
            var userId = CurrentUserId();


            var isFavorite =await _service.IsFavorite(userId, answerId);

            if (!isFavorite)
            {
               await _service.AddFavoriteAnswer(userId, answerId);
               return Json(new { isFavorite = true });

            }

            await _service.RemoveFavorite(userId, answerId);

            return Json(new { isFavorite = false });

        }

        private int CurrentUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(userId);
        }


    }
}
