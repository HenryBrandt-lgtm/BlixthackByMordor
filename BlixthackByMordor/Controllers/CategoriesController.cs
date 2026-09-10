using BlixthackByMordor.Models;
using BlixthackByMordor.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlixthackByMordor.Controllers
{
    public class CategoriesController(CategoryService categoryService) : Controller
    {
        private bool IsAdmin() => User.IsInRole("Admin");
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult CreateCategory()
        {
            if (!IsAdmin()) return Forbid();

            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateCategory(string categoryName)
        {
            var category = new CategoryModel()
            {
                Name = categoryName,
                CreatedAt = DateTime.Now

            };
            await categoryService.CreateNewCategory(category);
            return RedirectToAction("Index");

        }

    }
}
