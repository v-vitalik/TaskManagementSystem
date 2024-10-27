using TaskManagementSystem.Domain.Entities;

namespace TaskManagementSystem.Domain.Interfaces
{
    public interface ITaskRepository
    {
        Task<TaskEntity?> GetByIdAsync(int id);
        Task<int> AddAsync(TaskEntity task);
        Task<List<TaskEntity>> GetAllAsync();
        Task UpdateAsync(TaskEntity task);
        Task<bool> TaskWithNameExists(string name);
    }
}
