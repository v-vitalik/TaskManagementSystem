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

        public async Task<GetOperationResult<List<TaskDto>>> GetAllTasksAsync()
        {
            List<TaskEntity> tasks;
            try
            {
                tasks = await _taskRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all tasks");
                return new(OperationStatus.InternalError, ["Oops.. Something went wrong."]);
            }
            var dtos = tasks.Select(x => x.ToDto()).ToList();
            return new(dtos);
        }

        public async Task<AddOperationResult<int>> AddTaskAsync(TaskDto taskDto)
        {
            TaskEntity taskEntity = taskDto.ToEntity();
            taskEntity.Status = TaskEntityStatus.NotStarted;
            int taskId;
            try
            {
                taskId = await _taskRepository.AddAsync(taskEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add task, TaskDto: {@TaskDto}", taskDto);
                return new(OperationStatus.InternalError, ["Oops.. Something went wrong."]);
            }
            return new(taskId);
        }

        public async Task<OperationResult> UpdateTaskStatusAsync(int id, TaskEntityStatus newStatus)
        {
            TaskEntity entity;
            try
            {
                entity = await _taskRepository.GetByIdAsync(id);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Failed to get task by id during UpdateTaskStatusAsync execution, id = {@id}", id);
                return new(OperationStatus.InternalError, ["Oops.. Something went wrong."]);
            }

            if (entity == null)
            {
                return new(OperationStatus.NotFound, []);
            }

            entity.Status = newStatus;
            try
            {
                await _taskRepository.UpdateAsync(entity);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Failed to update task status, Id: {@TaskDto}", id);
                return new(OperationStatus.InternalError, ["Oops.. Something went wrong."]);
            }
            return new();
        }
    }
}
