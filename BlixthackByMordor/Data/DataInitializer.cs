using BlixthackByMordor.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace BlixthackByMordor.Data
{
    public class DataInitializer(ApplicationDbContext db, IConfiguration config)
    {
        public void MigrateAndSeedData()
        {
            db.Database.Migrate();
            SeedCategories();
            SeedAdminUser();
            db.SaveChanges();

            SeedThreads();
            db.SaveChanges();
        }

        public void SeedCategories()
        {
            var categoriesToSeed = new[] { "Technology", "Gaming", "Sports", "News", "Beer" };
            var existing = db.Categories.Select(c => c.Name).ToHashSet();

            foreach (var name in categoriesToSeed.Except(existing))
            {
                db.Categories.Add(new CategoryModel
                {
                    Name = name,
                    CreatedAt = DateTime.Now
                });
            }
        }
        public void SeedAdminUser()
        {
            if (!db.Users.Any(u => u.Username == "admin"))
            {
                var adminPw = config["AdminSeed:Password"]
                ?? throw new InvalidOperationException("AdminSeed:Password saknas i konfigurationen");

                var adminUser = new UserModel
                {
                    Username = "admin",
                    Email = "admin@example.com",
                    CreatedAt = DateTime.Now,
                    IsAdmin = true
                };
                var hasher = new PasswordHasher<UserModel>();
                adminUser.Password = hasher.HashPassword(adminUser, adminPw);
                db.Users.Add(adminUser);
            }
        }
        public void SeedThreads()
        {
            var admin = db.Users.First(u => u.Username == "admin");

            var threadsToSeed = new[]
            {
        new { Title = "Bästa GPU:erna 2026", Content = "Vad kör ni för grafikkort just nu, och är det värt att uppgradera?", Category = "Technology" },
        new { Title = "AI-agenter i vardagen", Content = "Hur mycket av ert arbete har AI tagit över hittills?", Category = "Technology" },

        new { Title = "Bästa spelen hittills i år", Content = "Vad har ni spelat mest av under 2026?", Category = "Gaming" },
        new { Title = "Näsata handkontroller", Content = "Är det värt att byta till en pro-controller eller räcker standard?", Category = "Gaming" },

        new { Title = "Gnaget", Content = "Kan ungdomarna i gnaget hämta hem guldet i år?(2026)", Category = "Sports" },
        new { Title = "VM-kval snackis", Content = "Hur ser ni på läget inför kvalet?", Category = "Sports" },
        new { Title ="Bajen, hur mycket kommer de darra på slutet?", Content = "Kommer bajen någonsin kunna ta ett guld, de klarar inte press?", Category = "Sports" },

        new { Title = "Dagens stora nyheter", Content = "Vad är det ingen pratar om men borde?", Category = "News" },
        new { Title = "Lokala nyheter vs rikstäckande", Content = "Följer ni mest lokala eller nationella nyhetskällor?", Category = "News" },

        new { Title = "Favoritölen just nu", Content = "Vad har ni druckit senast som imponerade?", Category = "Beer" },
        new { Title = "Hemmabryggning – värt besväret?", Content = "Någon här som bryggt sitt eget? Tips mottages!", Category = "Beer" },
    };

            foreach (var t in threadsToSeed)
            {
                if (!db.Threads.Any(th => th.Title == t.Title))
                {
                    var category = db.Categories.First(c => c.Name == t.Category);

                    db.Threads.Add(new ThreadModel
                    {
                        Title = t.Title,
                        Content = t.Content,
                        User = admin,
                        Category = category,
                        CreatedAt = DateTime.Now
                    });
                }
            }
        }
    }

}