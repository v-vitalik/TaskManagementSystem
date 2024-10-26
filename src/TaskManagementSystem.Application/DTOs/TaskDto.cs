using System.Text.Json.Serialization;
using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Application.DTOs
{
    public class TaskDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TaskEntityStatus? Status { get; set; }
        public string? AssignedTo { get; set; }
    }
}
