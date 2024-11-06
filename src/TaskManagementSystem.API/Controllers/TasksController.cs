using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.API.Extensions;
using TaskManagementSystem.API.Models;
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

        [HttpGet]
        public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
        {
            var result = await _taskService.GetAllTasksAsync(cancellationToken);
            return result.Status switch
            {
                OperationStatus.Success => Ok(result.Data),
                _ => StatusCode(500, new { result.Errors })
            };
        }

        [HttpPost]
        public async Task<IActionResult> AddTaskAsync([FromBody] AddTaskModel model, CancellationToken cancellationToken)
        {
            var result = await _taskService.AddTaskAsync(model.ToDto(), cancellationToken);
            return result.Status switch
            {
                OperationStatus.Success => Ok(new { result.Id }),
                OperationStatus.BadRequest => BadRequest(new { result.Errors }),
                _ => StatusCode(500, new { result.Errors })
            };
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateTaskStatusAsync(int id, 
            [FromBody] UpdateTaskStatusModel model, 
            CancellationToken cancellationToken)
        {
            var result = await _taskService.UpdateTaskStatusAsync(id, model.NewStatus, cancellationToken);
            return result.Status switch
            {
                OperationStatus.Success => Ok(),
                OperationStatus.NotFound => NotFound(),
                _ => StatusCode(500, new { result.Errors })
            };
        }
    }
}
