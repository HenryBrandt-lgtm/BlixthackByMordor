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
                .Include(thread => thread.Answers)

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

        public async Task<List<ThreadModel>> GetThreadsByCategoryId(int categoryId)
        {
            return await db.Threads
                .Include(thread => thread.User)
                .Include(thread => thread.Category)
                .Include(thread => thread.Answers)
                .Where(thread => thread.CategoryId == categoryId)
                .OrderByDescending(thread => thread.CreatedAt)
                .ToListAsync();
        }

        public async Task<ThreadModel?> GetThreadById(int id)
        {
            return await db.Threads
                .Include(thread => thread.User)
                .Include(thread => thread.Category)
                .FirstOrDefaultAsync(thread => thread.Id == id);
        }

        public async Task CreateThread(ThreadModel thread)
        {
            db.Threads.Add(thread);
            await db.SaveChangesAsync();
        }

        public async Task DeleteThread(ThreadModel thread)
        {
            db.Threads.Remove(thread);
            await db.SaveChangesAsync();
        }

        public async Task LockThread(ThreadModel thread, string username)
        {
            thread.ThreadLocked = true;
            thread.ThreadLockedAt = DateTime.Now;
            thread.ThreadLockedBy = username;

            await db.SaveChangesAsync();

        }

        public async Task UnlockThread(ThreadModel thread)
        {
            thread.ThreadLocked = false;
            thread.ThreadLockedAt = null;
            thread.ThreadLockedBy = null;


            await db.SaveChangesAsync();

        }


        public async Task UpdateThread(ThreadModel thread)
        {
            db.Threads.Update(thread);
            await db.SaveChangesAsync();
        }
    }
}
