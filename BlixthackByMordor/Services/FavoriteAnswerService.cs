using BlixthackByMordor.Data;
using BlixthackByMordor.Models;
using Microsoft.EntityFrameworkCore;

namespace BlixthackByMordor.Services
{
    public class FavoriteAnswerService
    {

        private readonly ApplicationDbContext _context;
        public FavoriteAnswerService(ApplicationDbContext context)
        {
            _context = context;

        }

        public async Task<bool> IsFavorite(int userId, int answerId)
        {
            return await _context.UserFavorite.AnyAsync(i => i.UserId == userId && i.AnswerId == answerId);
        }

        public async Task AddFavoriteAnswer(int userId, int answerId)
        {
            var favoriteAnswer = new UserFavoriteModel
            {
                UserId = userId,
                AnswerId = answerId,
                CreatedAt = DateTime.UtcNow,
            };

            _context.UserFavorite.Add(favoriteAnswer);
            await _context.SaveChangesAsync();
            
        }

        public async Task RemoveFavorite(int userId, int answerId)
        {
            var favoriteAnswer =
                await _context.UserFavorite.FirstOrDefaultAsync(i => i.UserId == userId && i.AnswerId == answerId);
            _context.UserFavorite.Remove(favoriteAnswer);
            await _context.SaveChangesAsync();

        }


    }
}
