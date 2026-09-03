using BlixthackByMordor.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlixthackByMordor.Controllers
{
    public class DatabaseController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DatabaseController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                await _context.Database.OpenConnectionAsync();
                await _context.Database.CloseConnectionAsync();

                return Content("✅ Databasen fungerar!");
            }
            catch (Exception ex)
            {
                return Content("❌ Databasen fungerar inte:\n\n" + ex.Message);
            }
        }
    }
}