using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManagementSystem.Application.DTOs;
using TaskManagementSystem.Application.Enums;
using TaskManagementSystem.Application.Services;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Enums;
using TaskManagementSystem.Domain.Interfaces;

namespace TaskManagementSystem.Application.Tests.Services
{
    public class TaskServiceTests
    {
        private Mock<ITaskRepository> _mockTaskRepository;
        private Mock<ILogger<TaskService>> _mockLogger;

        private TaskService _taskService;

        [SetUp]
        public void Setup()
        {
            _mockTaskRepository = new Mock<ITaskRepository>();
            _mockLogger = new Mock<ILogger<TaskService>>();

            _taskService = new TaskService(_mockTaskRepository.Object, _mockLogger.Object);
        }

        [Test]
        public async Task GetAllTasksAsync_ShouldReturnSuccessResultWithCorrectData_WhenNoErrorOccured()
        {
            // Arrange
            List<TaskEntity> expectedEntities = [
                new() 
                { 
                    Id = 1, 
                    Name = "name 1", 
                    Description = "test description", 
                    Status = TaskEntityStatus.InProgress, 
                    AssignedTo = "test" 
                }
            ];
            _mockTaskRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(expectedEntities);

            //Act
            var result = await _taskService.GetAllTasksAsync();

            //Assert
            result.Status.Should().Be(OperationStatus.Success);
            result.Data.Count.Should().Be(expectedEntities.Count);
            result.Data[0].Id.Should().Be(expectedEntities[0].Id);
            result.Data[0].Name.Should().Be(expectedEntities[0].Name);
            result.Data[0].Description.Should().Be(expectedEntities[0].Description);
            result.Data[0].Status.Should().Be(expectedEntities[0].Status);
            result.Data[0].AssignedTo.Should().Be(expectedEntities[0].AssignedTo);
        }

        [Test]
        public async Task GetAllTasksAsync_ShouldReturnInternalError_WhenErrorOccured()
        {
            // Arrange
            var expectedException = new Exception("Failed to get tasks");
            _mockTaskRepository.Setup(repo => repo.GetAllAsync())
                .ThrowsAsync(expectedException);

            //Act
            var result = await _taskService.GetAllTasksAsync();

            //Assert
            result.Status.Should().Be(OperationStatus.InternalError);
            result.Errors.Should().Contain("Oops.. Something went wrong.");
            _mockLogger.Verify(
                logger => logger.Log(
                    It.Is<LogLevel>(level => level == LogLevel.Error),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Failed to get all tasks")),
                    It.Is<Exception>(ex => ex == expectedException),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Test]
        public async Task AddTaskAsync_ShouldReturnSuccessResultWithTaskId_WhenNoErrorOccurred()
        {
            // Arrange
            int expectedTaskId = 1;
            _mockTaskRepository.Setup(repo => repo.AddAsync(It.IsAny<TaskEntity>()))
                .ReturnsAsync(expectedTaskId);
            var taskDto = new TaskDto
            {
                Name = "Task 1",
                Description = "Test description",
                Status = TaskEntityStatus.InProgress,
                AssignedTo = "User1"
            };

            // Act
            var result = await _taskService.AddTaskAsync(taskDto);

            // Assert
            result.Status.Should().Be(OperationStatus.Success);
            result.Id.Should().Be(expectedTaskId);
            _mockTaskRepository.Verify(repo => repo.AddAsync(It.Is<TaskEntity>(t =>
                t.Name == taskDto.Name &&
                t.Description == taskDto.Description &&
                t.Status == taskDto.Status &&
                t.AssignedTo == taskDto.AssignedTo
            )), Times.Once);
        }

        [Test]
        public async Task AddTaskAsync_ShouldReturnBadRequestResult_WhenNameAlreadyExists()
        {
            // Arrange
            var taskDto = new TaskDto
            {
                Name = "dublacate name",
                Description = "Test description",
                AssignedTo = "User1"
            };
            _mockTaskRepository.Setup(repo => repo.TaskWithNameExists(taskDto.Name)).ReturnsAsync(true);

            // Act
            var result = await _taskService.AddTaskAsync(taskDto);

            // Assert
            result.Status.Should().Be(OperationStatus.BadRequest);
        }

        [Test]
        public async Task AddTaskAsync_ShouldReturnInternalError_WhenErrorOccurred()
        {
            // Arrange
            var taskDto = new TaskDto
            {
                Name = "Task 1",
                Description = "Test description",
                Status = TaskEntityStatus.InProgress,
                AssignedTo = "User1"
            };
            var expectedException = new Exception("Database error");
            _mockTaskRepository.Setup(repo => repo.AddAsync(It.IsAny<TaskEntity>()))
                .ThrowsAsync(expectedException);

            // Act
            var result = await _taskService.AddTaskAsync(taskDto);

            // Assert
            result.Status.Should().Be(OperationStatus.InternalError);
            result.Errors.Should().Contain("Oops.. Something went wrong.");
            _mockLogger.Verify(
                logger => logger.Log(
                    It.Is<LogLevel>(level => level == LogLevel.Error),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Failed to add task")),
                    It.Is<Exception>(ex => ex == expectedException),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Test]
        public async Task UpdateTaskStatusAsync_ShouldReturnSuccessResult_WhenUpdateIsSuccessful()
        {
            // Arrange
            int taskId = 1;
            var taskEntity = new TaskEntity { Id = taskId, Status = TaskEntityStatus.NotStarted };
            var newStatus = TaskEntityStatus.Completed;

            _mockTaskRepository.Setup(repo => repo.GetByIdAsync(taskId)).ReturnsAsync(taskEntity);
            _mockTaskRepository.Setup(repo => repo.UpdateAsync(It.IsAny<TaskEntity>())).Returns(Task.CompletedTask);

            // Act
            var result = await _taskService.UpdateTaskStatusAsync(taskId, newStatus);

            // Assert
            result.Status.Should().Be(OperationStatus.Success);
            _mockTaskRepository.Verify(repo => repo.UpdateAsync(It.Is<TaskEntity>(t => t.Status == newStatus)), Times.Once);
        }

        [Test]
        public async Task UpdateTaskStatusAsync_ShouldReturnInternalError_WhenGetByIdThrowsException()
        {
            // Arrange
            int taskId = 1;
            var expectedException = new Exception("Database error");

            _mockTaskRepository.Setup(repo => repo.GetByIdAsync(taskId)).ThrowsAsync(expectedException);

            // Act
            var result = await _taskService.UpdateTaskStatusAsync(taskId, TaskEntityStatus.Completed);

            // Assert
            result.Status.Should().Be(OperationStatus.InternalError);
            result.Errors.Should().Contain("Oops.. Something went wrong.");
            _mockLogger.Verify(logger => logger.Log(
                It.Is<LogLevel>(level => level == LogLevel.Error),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Failed to get task by id during UpdateTaskStatusAsync execution")),
                It.Is<Exception>(ex => ex == expectedException),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
        }

        [Test]
        public async Task UpdateTaskStatusAsync_ShouldReturnNotFound_WhenEntityDoesNotExist()
        {
            // Arrange
            int taskId = 1;
            _mockTaskRepository.Setup(repo => repo.GetByIdAsync(taskId)).ReturnsAsync((TaskEntity)null);

            // Act
            var result = await _taskService.UpdateTaskStatusAsync(taskId, TaskEntityStatus.Completed);

            // Assert
            result.Status.Should().Be(OperationStatus.NotFound);
        }

        [Test]
        public async Task UpdateTaskStatusAsync_ShouldReturnInternalError_WhenUpdateThrowsException()
        {
            // Arrange
            int taskId = 1;
            var taskEntity = new TaskEntity { Id = taskId, Status = TaskEntityStatus.NotStarted };
            var expectedException = new Exception("Database error");

            _mockTaskRepository.Setup(repo => repo.GetByIdAsync(taskId)).ReturnsAsync(taskEntity);
            _mockTaskRepository.Setup(repo => repo.UpdateAsync(It.IsAny<TaskEntity>())).ThrowsAsync(expectedException);

            // Act
            var result = await _taskService.UpdateTaskStatusAsync(taskId, TaskEntityStatus.Completed);

            // Assert
            result.Status.Should().Be(OperationStatus.InternalError);
            result.Errors.Should().Contain("Oops.. Something went wrong.");
            _mockLogger.Verify(logger => logger.Log(
                It.Is<LogLevel>(level => level == LogLevel.Error),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Failed to update task status")),
                It.Is<Exception>(ex => ex == expectedException),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
        }
    }
}
