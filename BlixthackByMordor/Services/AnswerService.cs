using BlixthackByMordor.Data;
using BlixthackByMordor.Models;

namespace BlixthackByMordor.Services
{
    public class AnswerService(ApplicationDbContext db)
    {
        public async Task CreateAnswer(AnswerModel answer)
        {
            db.Answers.Add(answer);
            await db.SaveChangesAsync();
        }
    }
}
