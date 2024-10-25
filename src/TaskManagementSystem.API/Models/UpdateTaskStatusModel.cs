using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.API.Models
{
    public class UpdateTaskStatusModel
    {
        public TaskEntityStatus NewStatus { get; set; }
    }
}
