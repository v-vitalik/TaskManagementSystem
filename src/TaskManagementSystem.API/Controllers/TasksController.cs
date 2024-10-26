using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.API.Models;
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
        private readonly IServiceBusHandler _serviceBusHandler;

        public TasksController(ITaskService taskService, IServiceBusHandler serviceBusHandler)
        {
            _taskService = taskService;
            _serviceBusHandler = serviceBusHandler;
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

        [HttpPost]
        public async Task<IActionResult> AddTaskAsync([FromBody] TaskDto taskDto)
        {
            var result = await _taskService.AddTaskAsync(taskDto);
            return result.Status switch
            {
                OperationStatus.Success => Ok(new { result.Id }),
                OperationStatus.BadRequest => BadRequest(new { result.Errors }),
                _ => StatusCode(500, new { result.Errors })
            };
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateTaskStatusAsync(int id, [FromBody] UpdateTaskStatusModel model)
        {
            var result = await _taskService.UpdateTaskStatusAsync(id, model.NewStatus);
            return result.Status switch
            {
                OperationStatus.Success => Ok(),
                OperationStatus.NotFound => NotFound(),
                _ => StatusCode(500, new { result.Errors })
            };
        }

        [HttpPost("message")]
        public async Task<IActionResult> SendMessage([FromBody] Message message)
        {
            await _serviceBusHandler.SendMessage(message);
            return Ok();
        }

        [HttpGet("cons")]
        public IActionResult Consume()
        {
            _serviceBusHandler.ReceiveMessage<Message>();
            return Ok();
        }
    }

    public class Message
    {
        public string Content { get; set; }
    }
}
