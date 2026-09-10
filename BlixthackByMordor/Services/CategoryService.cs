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

        public async Task CreateNewCategory(CategoryModel category)
        {

            db.Categories.Add(category);
            await db.SaveChangesAsync();
        }

        public async Task<bool> CategoryExists(string categoryName)
        {
            return await db.Categories.AnyAsync(c => c.Name == categoryName);
        }

    }
}
