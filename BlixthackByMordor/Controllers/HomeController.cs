using BlixthackByMordor.Data;
using BlixthackByMordor.Models;
using BlixthackByMordor.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BlixthackByMordor.Controllers
{
    public class HomeController(ApplicationDbContext db) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var threads = await new ThreadService(db).GetLatestThreadsPerCategory();
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
