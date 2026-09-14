using BlixthackByMordor.Data;
using BlixthackByMordor.Models;
using Microsoft.EntityFrameworkCore;

namespace BlixthackByMordor.Services
{
    public class AnswerService(ApplicationDbContext db)
    {
        public async Task CreateAnswer(AnswerModel answer)
        {
            db.Answers.Add(answer);
            await db.SaveChangesAsync();
        }
        public async Task DeleteAnswer(AnswerModel answer)
        {
            db.Answers.Remove(answer);
            await db.SaveChangesAsync();
        }
        public async Task<AnswerModel?> GetAnswerById(int id)
        {
            return await db.Answers.FirstOrDefaultAsync(a => a.Id == id);
        }
        public async Task<(List<AnswerModel> Answers, int TotalCount)> GetAnswersForThread(int threadId, int page, int pageSize = 10)
        {
            var query = db.Answers
                .Where(a => a.ThreadId == threadId && a.ReplyId == null)
                .OrderBy(a => a.CreatedAt);

            var totalCount = await query.CountAsync();

            var answers = await query
                .Include(a => a.User)
                .Include(a => a.Favorites)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (answers, totalCount);
        }

        public async Task<List<AnswerModel>> GetRepliesForAnswer(int answerId)
        {

            var answers = await db.Answers
                .Where(a => a.ReplyId == answerId)
                .Include(a => a.User)
                .ToListAsync();

            return (answers);
        }




    }

}


