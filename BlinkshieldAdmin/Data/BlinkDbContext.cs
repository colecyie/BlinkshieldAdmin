using BlinkshieldAdmin.Models;
using Microsoft.EntityFrameworkCore;

namespace BlinkshieldAdmin.Data
{
    public class BlinkDbContext : DbContext
    {
        public BlinkDbContext(DbContextOptions<BlinkDbContext> options) : base(options)
        {
        }

        public DbSet<AdminAccount> AdminAccounts { get; set; }
        public DbSet<AdminHistory> AdminHistories { get; set; }
        public DbSet<Achievement> Achievements { get; set; }
        public DbSet<EyeTrivia> EyeTrivia { get; set; }
        public DbSet<EyeCareTip> EyeCareTip { get; set; }
        public DbSet<EyeIssue> EyeIssues { get; set; }
        public DbSet<WardrobeItem> WardrobeItems { get; set; }
        public DbSet<Feedback> Feedback { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ✅ EyeTrivia mapping
            modelBuilder.Entity<EyeTrivia>(entity =>
            {
                entity.ToTable("eye_trivias");
                entity.HasKey(e => e.TriviaId).HasName("PK_eye_trivias");
                entity.Property(e => e.TriviaId).HasColumnName("trivia_id");
            });

            // ✅ WardrobeItem mapping
            modelBuilder.Entity<WardrobeItem>()
                .Property(e => e.Type)
                .HasConversion<string>();
        }
    }
}
