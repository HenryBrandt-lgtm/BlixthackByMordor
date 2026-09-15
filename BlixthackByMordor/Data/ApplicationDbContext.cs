using Microsoft.EntityFrameworkCore;
using BlixthackByMordor.Models;

namespace BlixthackByMordor.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<AnswerModel> Answers { get; set; }
        public DbSet<CategoryModel> Categories { get; set; }
        public DbSet<ThreadModel> Threads { get; set; }
        public DbSet<UserModel> Users { get; set; }
        public DbSet<UserFavoriteModel> UserFavorite { get; set; }

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AnswerModel>()
                .HasOne(a => a.User)
                .WithMany(u => u.Answers)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // Favorite -> User
            modelBuilder.Entity<UserFavoriteModel>()
                .HasOne(f => f.User)
                .WithMany(u => u.Favorites)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Favorite -> Answer
            modelBuilder.Entity<UserFavoriteModel>()
                .HasOne(f => f.Answer)
                .WithMany(a => a.Favorites)
                .HasForeignKey(f => f.AnswerId)
                .OnDelete(DeleteBehavior.NoAction);


        }
    }
}
