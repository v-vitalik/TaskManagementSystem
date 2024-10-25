using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.DataAccess.Configuration;
using TaskManagementSystem.Domain.Entities;

namespace TaskManagementSystem.DataAccess
{
    internal class TasksDbContext : DbContext
    {
        public TasksDbContext(DbContextOptions<TasksDbContext> options) : base(options)
        {
        }

        public DbSet<TaskEntity> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new TaskEntityConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
