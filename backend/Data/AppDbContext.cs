using Microsoft.EntityFrameworkCore;
using WikiVideojuegos.Models;

namespace WikiVideojuegos.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Game> Games => Set<Game>();
    public DbSet<Character> Characters => Set<Character>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
        modelBuilder.Entity<Character>()
            .HasOne(c => c.Game)
            .WithMany(g => g.Characters)
            .HasForeignKey(c => c.GameId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
