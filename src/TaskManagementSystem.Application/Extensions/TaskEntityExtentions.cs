using TaskManagementSystem.Application.DTOs;
using TaskManagementSystem.Domain.Entities;

namespace TaskManagementSystem.Application.Extensions
{
    public static class TaskEntityExtentions
    {
        public static TaskDto ToDto(this TaskEntity taskEntity)
        {
            return new TaskDto
            {
                Id = taskEntity.Id,
                Name = taskEntity.Name,
                Description = taskEntity.Description,
                Status = taskEntity.Status,
                AssignedTo = taskEntity.AssignedTo
            };
        }
    }
}
