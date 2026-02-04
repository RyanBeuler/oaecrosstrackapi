using Microsoft.EntityFrameworkCore;
using OaeCrosstrackApi.Models;

namespace OaeCrosstrackApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Athlete> Athletes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Make username unique
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // Configure Athlete entity
            modelBuilder.Entity<Athlete>()
                .HasIndex(a => new { a.FirstName, a.LastName, a.GraduationYear })
                .IsUnique();
        }
    }
}