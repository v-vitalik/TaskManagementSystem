using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Application.DTOs;
using TaskManagementSystem.Application.Enums;
using TaskManagementSystem.Application.Services;

namespace TaskManagementSystem.API.Controllers
{
    [Route("api/tasks")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpPost]
        public async Task<IActionResult> AddTaskAsync([FromBody] TaskDto taskDto)
        {
            var result = await _taskService.AddTaskAsync(taskDto);
            return result.Status switch
            {
                OperationStatus.Success => Ok(result.Id),
                OperationStatus.BadRequest => BadRequest(new { result.Errors }),
                _ => StatusCode(500, new { result.Errors })
            };
        }

        [HttpPut]
        public async Task<IActionResult> UpdateTaskAsync([FromBody] TaskDto taskDto)
        {
            var result = await _taskService.UpdateTaskAsync(taskDto);
            return result.Status switch
            {
                OperationStatus.Success => Ok(),
                OperationStatus.NotFound => NotFound(),
                _ => StatusCode(500, new { result.Errors })
            };
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await _taskService.GetAllTasksAsync();
            return result.Status switch
            {
                OperationStatus.Success => Ok(result.Data),
                _ => StatusCode(500, new { result.Errors })
            };
        }
    }
}
