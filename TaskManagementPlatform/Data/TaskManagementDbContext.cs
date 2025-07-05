using Microsoft.EntityFrameworkCore;
using TaskManagementPlatform.Models;

namespace TaskManagementPlatform.Data
{
    public class TaskManagementDbContext : DbContext
    {
        public TaskManagementDbContext(DbContextOptions<TaskManagementDbContext> ops) : base(ops)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<AppTask> Tasks { get; set; }
        public DbSet<TaskStatusHistory> TaskStatusHistories { get; set; }
        public DbSet<TaskCustomField> TaskCustomFields { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.Name).IsRequired().HasMaxLength(100);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(100);
            });

            // Configure Task
            modelBuilder.Entity<AppTask>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Title).IsRequired().HasMaxLength(100);
                entity.Property(t => t.TaskType).IsRequired().HasMaxLength(50);
                entity.Property(t => t.CurrentStatus).IsRequired();
                entity.Property(t => t.IsClosed).IsRequired();
                entity.Property(t => t.CreatedAt).IsRequired();

                // Foreign key relationship
                entity.HasOne(t => t.AssignedUser)
                      .WithMany(u => u.AssignedTasks)
                      .HasForeignKey(t => t.AssignedUserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure TaskStatusHistory
            modelBuilder.Entity<TaskStatusHistory>(entity =>
            {
                entity.HasKey(h => h.Id);
                entity.Property(h => h.ChangedAt).IsRequired();
                entity.Property(h => h.Notes).HasMaxLength(500);

                // Foreign key relationships
                entity.HasOne(h => h.Task)
                      .WithMany(t => t.StatusHistory)
                      .HasForeignKey(h => h.TaskId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(h => h.AssignedUser)
                      .WithMany()
                      .HasForeignKey(h => h.AssignedUserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure TaskCustomField
            modelBuilder.Entity<TaskCustomField>(entity =>
            {
                entity.HasKey(f => f.Id);
                entity.Property(f => f.FieldName).IsRequired().HasMaxLength(100);
                entity.Property(f => f.FieldValue).IsRequired();
                entity.Property(f => f.FieldType).HasMaxLength(50);
                entity.Property(f => f.CreatedAt).IsRequired();

                // Foreign key relationship
                entity.HasOne(f => f.appTask)
                      .WithMany(t => t.CustomFields)
                      .HasForeignKey(f => f.TaskId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Composite index for performance
                entity.HasIndex(f => new { f.TaskId, f.FieldName });
            });

            // Seed data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Users
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Name = "Dani Lev", Email = "DaniLev@company.com" },
                new User { Id = 2, Name = "Adma ben", Email = "Admaben@company.com" },
                new User { Id = 3, Name = "Avi gal", Email = "avigal@company.com" },
                new User { Id = 4, Name = "Ban dale", Email = "bandale@company.com" }
            );
        }

    }
}
