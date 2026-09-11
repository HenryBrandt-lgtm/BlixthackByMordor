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
        public async Task<IActionResult> CreateCategory()
        {
            if (!IsAdmin()) return Forbid();


            // CreateCategory fortsätter styra Viewn, men hämtar alla kategorier
            // så att Edit-sektionen kan använda dem i dropdown-menyn.
            var categories = await categoryService.GetCategories();

            return View(categories);
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


            TempData["SuccessMessage"] = $"Category {category.Name} created successfully";

            return RedirectToAction("Index", "Home");

        }

      


    }
}
