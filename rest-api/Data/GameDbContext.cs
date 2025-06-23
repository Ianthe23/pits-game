using Microsoft.EntityFrameworkCore;
using RestAPI.Models;

namespace RestAPI.Data
{
    public class GameDbContext : DbContext
    {
        public GameDbContext(DbContextOptions<GameDbContext> options) : base(options)
        {
        }

        public DbSet<Player> Players { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<PitElement> PitElements { get; set; }
        public DbSet<GameAttempt> GameAttempts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Player>()
                .HasIndex(p => p.Name)
                .IsUnique();

            modelBuilder.Entity<Game>()
                .HasOne(g => g.Player)
                .WithMany(p => p.Games)
                .HasForeignKey(g => g.PlayerId);

            modelBuilder.Entity<PitElement>()
                .HasOne(p => p.Game)                    // Each pit belongs to one game
                .WithMany(g => g.PitElements)           // Each game has many pits
                .HasForeignKey(p => p.GameId)           // Foreign key
                .OnDelete(DeleteBehavior.Cascade);  

            modelBuilder.Entity<GameAttempt>()
                .HasOne(a => a.Game)
                .WithMany(g => g.Attempts)
                .HasForeignKey(a => a.GameId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
