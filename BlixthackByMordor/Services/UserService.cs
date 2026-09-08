using BlixthackByMordor.Data;
using BlixthackByMordor.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlixthackByMordor.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;
        public UserService(ApplicationDbContext context)
        {
            _context = context;

        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users
                .AnyAsync(u => u.Email == email);
        }

        public async Task<UserModel?> RegisterAsync(string username,string email,string password)
        {
            // Kontrollera om email redan finns
            if (await EmailExistsAsync(email))
            {
                return null;
            }

            // Skapa användaren
            var user = new UserModel
            {
                Username = username,
                Email = email,
                CreatedAt = DateTime.UtcNow,
                Password = password
            };



            // Lägg till användaren i databasen
            _context.Users.Add(user);

            // Spara
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<UserModel?> LoginAsync(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
            return user;
        }



    }
}
