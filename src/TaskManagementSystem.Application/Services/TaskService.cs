using Microsoft.Extensions.Logging;
using TaskManagementSystem.Application.DTOs;
using TaskManagementSystem.Application.Enums;
using TaskManagementSystem.Application.Extensions;
using TaskManagementSystem.Application.Results;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Enums;
using TaskManagementSystem.Domain.Interfaces;

namespace TaskManagementSystem.Application.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly ILogger<TaskService> _logger;

        public TaskService(ITaskRepository taskRepository, ILogger<TaskService> logger)
        {
            _taskRepository = taskRepository;
            _logger = logger;
        }

        public async Task<GetOperationResult<List<TaskDto>>> GetAllTasksAsync(CancellationToken cancellationToken)
        {
            List<TaskEntity> tasks = await _taskRepository.GetAllAsync(cancellationToken);
            var dtos = tasks.Select(x => x.ToDto()).ToList();
            return new(dtos);
        }

        public async Task<AddOperationResult<int>> AddTaskAsync(TaskDto taskDto, CancellationToken cancellationToken)
        {
            TaskEntity taskEntity = taskDto.ToEntity();
            taskEntity.Status = taskDto.Status ?? TaskEntityStatus.NotStarted;

            if (await _taskRepository.TaskWithNameExists(taskEntity.Name, cancellationToken))
            {
                return new(OperationStatus.BadRequest, 
                    [$"Task with Name = {taskEntity.Name} already exist. Please enter unique name."]);
            }

            int taskId = await _taskRepository.AddAsync(taskEntity, cancellationToken);

            return new(taskId);
        }

        public async Task<OperationResult> UpdateTaskStatusAsync(int id, TaskEntityStatus newStatus, CancellationToken cancellationToken)
        {
            TaskEntity entity = await _taskRepository.GetByIdAsync(id, cancellationToken);
            if (entity == null)
            {
                return new(OperationStatus.NotFound, []);
            }

            entity.Status = newStatus;
            await _taskRepository.UpdateAsync(entity, cancellationToken);

            return new();
        }
    }
}
