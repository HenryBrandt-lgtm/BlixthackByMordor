using BlixthackByMordor.Data;
using BlixthackByMordor.Models;
using Microsoft.EntityFrameworkCore;

namespace BlixthackByMordor.Services
{
    public class ThreadService(ApplicationDbContext db)
    {
        public async Task<List<ThreadModel>> GetAllThreads()
        {
            return await db.Threads
                .Include(thread => thread.User)
                .Include(thread => thread.Category)
                .OrderBy(thread => thread.Category.Name)
                .ThenByDescending(thread => thread.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<ThreadModel>> GetLatestThreadsPerCategory(int limit = 10)
        {
            var threads = await GetAllThreads();

            return threads
                .GroupBy(thread => thread.Category.Name)
                .OrderBy(group => group.Key)
                .SelectMany(group => group.Take(limit))
                .ToList();
        }

        public async Task<ThreadModel?> GetThreadById(int id)
        {
            return await db.Threads
                .Include(thread => thread.User)
                .Include(thread => thread.Category)
                .Include(thread => thread.Answers!.OrderBy(answer => answer.CreatedAt))
                    .ThenInclude(answer => answer.User)
                .FirstOrDefaultAsync(thread => thread.Id == id);
        }

        public async Task CreateThread(ThreadModel thread)
        {
            db.Threads.Add(thread);
            await db.SaveChangesAsync();
        }
    }
}
