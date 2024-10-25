using TaskManagementSystem.Application.DTOs;
using TaskManagementSystem.Application.Results;
using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Application.Services
{
    public interface ITaskService
    {
        Task<GetOperationResult<List<TaskDto>>> GetAllTasksAsync();
        Task<AddOperationResult<int>> AddTaskAsync(TaskDto taskDto);
        Task<OperationResult> UpdateTaskStatusAsync(int id, TaskEntityStatus newStatus);
    }
}
