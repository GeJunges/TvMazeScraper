using Microsoft.EntityFrameworkCore;
using TvMazeScraper.Domain.Model;

namespace TvMazeScraper.Infrastructure.Settings
{
    public class TvMazeScraperContext : DbContext
    {
        public TvMazeScraperContext(DbContextOptions<TvMazeScraperContext> options) : base(options) { }
        public DbSet<Show> Shows { get; set; }
        public DbSet<Cast> Casts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Show>().HasIndex(i => i.ApiId).ForSqlServerIsClustered(false);
            modelBuilder.Entity<Cast>();
        }
    }
}
