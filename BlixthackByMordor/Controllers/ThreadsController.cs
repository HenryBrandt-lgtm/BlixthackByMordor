<<<<<<< HEAD
﻿using BlixthackByMordor.Data;
using BlixthackByMordor.Services;
=======
﻿using BlixthackByMordor.Services;
>>>>>>> 64a73c4 (feat: Add drop down menu for Categorys.)
using Microsoft.AspNetCore.Mvc;

namespace BlixthackByMordor.Controllers
{
<<<<<<< HEAD
    
=======
    public class ThreadsController(CategoryService categoryService, ThreadService threadService) : Controller
>>>>>>> 64a73c4 (feat: Add drop down menu for Categorys.)
    {
        public IActionResult Index()
        {
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpGet("/Threads/{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            var thread = await threadService.GetThreadById(id);
            if (thread == null)
            {
                return NotFound();
            }

            return View(thread);
        }


        public async Task<IActionResult> CreateThread()
        {
            var categories = await categoryService.GetCategories();

            return View(categories);
        }
    }
}
