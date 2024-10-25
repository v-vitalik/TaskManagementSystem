using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Interfaces;

namespace TaskManagementSystem.DataAccess.Repositories
{
    internal class TaskRepository : ITaskRepository
    {
        private readonly TasksDbContext _dbContext;
        public TaskRepository(TasksDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TaskEntity?> GetByIdAsync(int id)
        {
            return await _dbContext.Tasks.FindAsync(id);
        }

        public async Task<int> AddAsync(TaskEntity task)
        {
            _dbContext.Tasks.Add(task);
            await _dbContext.SaveChangesAsync();
            return task.Id;
        }

        public async Task<List<TaskEntity>> GetAllAsync()
        {
            return await _dbContext.Tasks.AsNoTracking().ToListAsync();
        }

        public async Task UpdateAsync(TaskEntity task)
        {
            _dbContext.Tasks.Update(task);
            await _dbContext.SaveChangesAsync();
        }
    }
}
