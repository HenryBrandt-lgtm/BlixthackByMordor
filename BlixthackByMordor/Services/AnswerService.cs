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
    }
}

