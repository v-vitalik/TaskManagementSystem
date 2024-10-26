﻿using TaskManagementSystem.Application.DTOs;
using TaskManagementSystem.Domain.Entities;

namespace TaskManagementSystem.Application.Extensions
{
    public static class TaskDtoExtensions
    {
        public static TaskEntity ToEntity(this TaskDto taskDto)
        {
            return new TaskEntity
            {
                Id = taskDto.Id,
                Name = taskDto.Name,
                Description = taskDto.Description,
                AssignedTo = taskDto.AssignedTo
            };
        }
    }
}
