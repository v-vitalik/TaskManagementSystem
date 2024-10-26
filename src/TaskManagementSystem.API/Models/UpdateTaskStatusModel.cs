using System.Text.Json.Serialization;
using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.API.Models
{
    public class UpdateTaskStatusModel
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TaskEntityStatus NewStatus { get; set; }
    }
}
