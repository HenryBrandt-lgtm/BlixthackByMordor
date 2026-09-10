using BlixthackByMordor.Models;
using BlixthackByMordor.Services;
using BlixthackByMordor.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BlixthackByMordor.Controllers
{
    public class HomeController(ThreadService threadService, CategoryService categoryService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var threads = await threadService.GetLatestThreadsPerCategory();
            var categories = await categoryService.GetCategories();

            var model = new HomeViewModel
            {
                Threads = threads,
                Categories = categories
            };
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
