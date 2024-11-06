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

        public async Task<TaskEntity?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _dbContext.Tasks.FindAsync(id, cancellationToken);
        }

        public async Task<int> AddAsync(TaskEntity task, CancellationToken cancellationToken)
        {
            _dbContext.Tasks.Add(task);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return task.Id;
        }

        public async Task<List<TaskEntity>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _dbContext.Tasks.AsNoTracking().ToListAsync(cancellationToken);
        }

        public async Task UpdateAsync(TaskEntity task, CancellationToken cancellationToken)
        {
            _dbContext.Tasks.Update(task);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> TaskWithNameExists(string name, CancellationToken cancellationToken)
        {
            return await _dbContext.Tasks.AnyAsync(x => x.Name == name, cancellationToken);
        }
    }
}
