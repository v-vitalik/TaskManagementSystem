using TaskManagementSystem.API.Models;
using TaskManagementSystem.Application.DTOs;

namespace TaskManagementSystem.API.Extensions
{
    public static class AddTaskModelExtensions
    {
        public static TaskDto ToDto(this AddTaskModel model)
        {
            return new TaskDto
            {
                Name = model.Name,
                Description = model.Description,
                Status = model.Status,
                AssignedTo = model.AssignedTo
            };
        }
    }
}
