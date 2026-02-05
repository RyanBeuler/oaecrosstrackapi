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
        public DbSet<Sport> Sports { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<RosterEntry> RosterEntries { get; set; }
        public DbSet<Meet> Meets { get; set; }
        public DbSet<Result> Results { get; set; }
        public DbSet<Record> Records { get; set; }

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

            // Configure Sport entity
            modelBuilder.Entity<Sport>()
                .HasIndex(s => s.Name)
                .IsUnique();

            // Configure Event entity
            modelBuilder.Entity<Event>()
                .HasIndex(e => new { e.SportId, e.Name })
                .IsUnique();

            // Configure Event-Sport relationship
            modelBuilder.Entity<Event>()
                .HasOne(e => e.Sport)
                .WithMany(s => s.Events)
                .HasForeignKey(e => e.SportId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure RosterEntry entity - unique constraint on Athlete+Sport+Year
            modelBuilder.Entity<RosterEntry>()
                .HasIndex(r => new { r.AthleteId, r.SportId, r.Year })
                .IsUnique();

            // Configure RosterEntry relationships
            modelBuilder.Entity<RosterEntry>()
                .HasOne(r => r.Athlete)
                .WithMany()
                .HasForeignKey(r => r.AthleteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RosterEntry>()
                .HasOne(r => r.Sport)
                .WithMany()
                .HasForeignKey(r => r.SportId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Meet entity
            modelBuilder.Entity<Meet>()
                .HasOne(m => m.Sport)
                .WithMany()
                .HasForeignKey(m => m.SportId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Result entity - unique constraint on Athlete+Meet+Event
            modelBuilder.Entity<Result>()
                .HasIndex(r => new { r.AthleteId, r.MeetId, r.EventId })
                .IsUnique();

            // Configure Result relationships
            modelBuilder.Entity<Result>()
                .HasOne(r => r.Athlete)
                .WithMany()
                .HasForeignKey(r => r.AthleteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Result>()
                .HasOne(r => r.Meet)
                .WithMany()
                .HasForeignKey(r => r.MeetId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Result>()
                .HasOne(r => r.Event)
                .WithMany()
                .HasForeignKey(r => r.EventId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Record entity
            modelBuilder.Entity<Record>()
                .HasOne(r => r.Event)
                .WithMany()
                .HasForeignKey(r => r.EventId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Record>()
                .HasOne(r => r.Athlete)
                .WithMany()
                .HasForeignKey(r => r.AthleteId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}