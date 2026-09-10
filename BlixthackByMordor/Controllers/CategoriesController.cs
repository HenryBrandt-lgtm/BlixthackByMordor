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
            if (string.IsNullOrWhiteSpace(categoryName))
            {
                ModelState.AddModelError("categoryName", "Category name cannot be empty.");
                return View();
            }
            if (await categoryService.CategoryExists(categoryName))
            {
                ModelState.AddModelError("categoryName", "Category already exists.");
                return View();
            }

            var category = new CategoryModel()
            {
                Name = categoryName,
                CreatedAt = DateTime.Now

            };
            await categoryService.CreateNewCategory(category);
            return RedirectToAction("Index", "Home");

        }
        [Authorize]
        [HttpGet("/Categories/{id:int}/Edit")]
        public async Task<IActionResult> EditCategory(int id)
        {
            if (!IsAdmin()) return Forbid();

            var category = await categoryService.GetCategoryById(id);
            if (category == null) return NotFound();

            return View("EditCategory", category);
        }
        [Authorize]
        [HttpPost("/Categories/{id:int}/Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory(int id, string categoryName)
        {
            if (!IsAdmin()) return Forbid();

            var category = await categoryService.GetCategoryById(id);
            if (category == null) return NotFound();

            if (string.IsNullOrWhiteSpace(categoryName))
            {
                ModelState.AddModelError("categoryName", "Category name cannot be empty.");
                return View("EditCategory", category);
            }

            category.Name = categoryName;
            await categoryService.UpdateCategory(category);

            return RedirectToAction("Index", "Home");
        }

    }
}
