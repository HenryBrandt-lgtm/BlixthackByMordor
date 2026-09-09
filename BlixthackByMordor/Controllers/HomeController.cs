using BlixthackByMordor.Models;
using BlixthackByMordor.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BlixthackByMordor.Controllers
{
    public class HomeController(ThreadService threadService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var threads = await threadService.GetLatestThreadsPerCategory();
            return View(threads);
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
