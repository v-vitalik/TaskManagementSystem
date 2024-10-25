using TaskManagementSystem.Application.DTOs;
using TaskManagementSystem.Application.Results;

namespace TaskManagementSystem.Application.Services
{
    public interface ITaskService
    {
        Task<AddOperationResult<int>> AddTaskAsync(TaskDto taskDto);
        Task<OperationResult> UpdateTaskAsync(TaskDto taskDto);
        Task<GetOperationResult<List<TaskDto>>> GetAllTasksAsync();
    }
}
