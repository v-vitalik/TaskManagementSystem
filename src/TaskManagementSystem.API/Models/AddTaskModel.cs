using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.API.Models
{
    public class AddTaskModel
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [Required]
        [MaxLength(500)]
        public string Description { get; set; }
        [Range(0, 2, ErrorMessage = $"NewStatus should be between 0 and 2 or one of ['NotStarted', 'InProgress', 'Completed']")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TaskEntityStatus? Status { get; set; }
        [MaxLength(100)]
        public string? AssignedTo { get; set; }
    }
}
