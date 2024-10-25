using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Application.DTOs
{
    public class TaskDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Domain.Enums.TaskEntityStatus? Status { get; set; }
        public string AssignedTo { get; set; }
    }
}
