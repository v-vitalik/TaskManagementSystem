﻿using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.API.Models
{
    public class UpdateTaskStatusModel
    {
        [Required]
        [Range(0, 2, ErrorMessage = $"NewStatus should be between 0 and 2 or one of ['NotStarted', 'InProgress', 'Completed']")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TaskEntityStatus NewStatus { get; set; }
    }
}
