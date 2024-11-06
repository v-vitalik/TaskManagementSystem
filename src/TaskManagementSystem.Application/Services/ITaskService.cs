using TaskManagementSystem.Application.DTOs;
using TaskManagementSystem.Application.Results;
using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Application.Services
{
    public interface ITaskService
    {
        Task<GetOperationResult<List<TaskDto>>> GetAllTasksAsync(CancellationToken cancellationToken);
        Task<AddOperationResult<int>> AddTaskAsync(TaskDto taskDto, CancellationToken cancellationToken);
        Task<OperationResult> UpdateTaskStatusAsync(int id, TaskEntityStatus newStatus, CancellationToken cancellationToken);
    }
}
