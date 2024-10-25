using TaskManagementSystem.Application.DTOs;
using TaskManagementSystem.Application.Enums;
using TaskManagementSystem.Application.Extensions;
using TaskManagementSystem.Application.Results;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Interfaces;

namespace TaskManagementSystem.Application.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        public TaskService(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<AddOperationResult<int>> AddTaskAsync(TaskDto taskDto)
        {
            TaskEntity taskEntity = taskDto.ToEntity();
            taskEntity.Status = Domain.Enums.Status.NotStarted;
            int taskId;
            try
            {
                taskId = await _taskRepository.AddAsync(taskEntity);
            }
            catch (Exception ex)
            {
                return new(OperationStatus.InternalError, ["Oops.. Something went wrong."]);
            }
            return new(taskId);
        }

        public async Task<OperationResult> UpdateTaskAsync(TaskDto taskDto)
        {
            var entity = await _taskRepository.GetByIdAsync(taskDto.Id);
            if (entity == null) 
            {
                return new(OperationStatus.NotFound, []);
            }

            taskDto.MapDtoToEntity(entity);
            try
            {
                await _taskRepository.UpdateAsync(entity);
            }
            catch
            {
                return new(OperationStatus.InternalError, ["Oops.. Something went wrong."]);
            }
            return new();
        }

        public async Task<GetOperationResult<List<TaskDto>>> GetAllTasksAsync()
        {
            List<TaskEntity> tasks;
            try
            {
                tasks = await _taskRepository.GetAllAsync();
            }
            catch
            {
                return new(OperationStatus.InternalError, ["Oops.. Something went wrong."]);
            }
            var dtos =  tasks.Select(x => x.ToDto()).ToList();
            return new(dtos);
        }
    }
}
