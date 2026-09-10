using BlixthackByMordor.Services;
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


        //public async Task<IActionResult> Add(int postId)
        //{
        //    var userId = CurrentUserId();

        //}

        private int? CurrentUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return userId == null ? null : int.Parse(userId);
        }


    }
}
