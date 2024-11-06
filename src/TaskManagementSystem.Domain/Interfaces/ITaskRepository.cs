using TaskManagementSystem.Domain.Entities;

namespace TaskManagementSystem.Domain.Interfaces
{
    public interface ITaskRepository
    {
        Task<TaskEntity?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<int> AddAsync(TaskEntity task, CancellationToken cancellationToken);
        Task<List<TaskEntity>> GetAllAsync(CancellationToken cancellationToken);
        Task UpdateAsync(TaskEntity task, CancellationToken cancellationToken);
        Task<bool> TaskWithNameExists(string name, CancellationToken cancellationToken);
    }
}
