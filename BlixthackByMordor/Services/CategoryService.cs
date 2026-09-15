using BlixthackByMordor.Data;
using BlixthackByMordor.Models;
using Microsoft.EntityFrameworkCore;

namespace BlixthackByMordor.Services
{
    public class CategoryService(ApplicationDbContext db)
    {
        public async Task<List<CategoryModel>> GetCategories()
        {
            return await db.Categories
                .OrderBy(category => category.Name)
                .ToListAsync();
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

        public async Task<CategoryModel?> GetCategoryById(int id)
        {
            return await db.Categories.FirstOrDefaultAsync(c => c.Id == id);


        }

        public async Task UpdateCategory(CategoryModel category)
        {
            db.Categories.Update(category);
            await db.SaveChangesAsync();
        }

    }
}
