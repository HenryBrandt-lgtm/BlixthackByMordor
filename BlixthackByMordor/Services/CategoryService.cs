using BlixthackByMordor.Data;
using BlixthackByMordor.Models;
using Microsoft.EntityFrameworkCore;

namespace BlixthackByMordor.Services
{
    public class CategoryService(ApplicationDbContext db)
    {
        public async Task<List<CategoryModel>> GetCategories()
        {
            return await db.Categories.ToListAsync();
        }

    }
}
