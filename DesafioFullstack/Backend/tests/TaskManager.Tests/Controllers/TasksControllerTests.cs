using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManager.Application.DTOs.Tasks;
using TaskManager.Application.Interfaces.Services;
using TaskManager.WebApi.Controllers;

namespace TaskManager.Tests.Controllers
{
    public class TasksControllerTests
    {
        private readonly Mock<ITaskService> _taskServiceMock;
        private readonly TasksController _controller;

        public TasksControllerTests()
        {
            _taskServiceMock = new Mock<ITaskService>();

            _controller = new TasksController(_taskServiceMock.Object);

            SetupAuthenticatedUser();
        }

        private void SetupAuthenticatedUser()
        {
            var userId = Guid.NewGuid();

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userId.ToString())
            };

            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = principal
                }
            };
        }

        [Fact]
        public async Task GetAll_ShouldReturnOk_WithTasks()
        {
            var tasks = new List<TaskResponseDto>
        {
            new() { Title = "Task 1" },
            new() { Title = "Task 2" }
        };

            _taskServiceMock
                .Setup(s => s.GetAllAsync(
                    It.IsAny<Guid>(),
                    null))
                .ReturnsAsync(tasks);

            var result = await _controller.GetAll(null);

            result.Should().BeOfType<OkObjectResult>();

            var okResult = result as OkObjectResult;

            okResult!.Value.Should().Be(tasks);

            _taskServiceMock.Verify(
                s => s.GetAllAsync(
                    It.IsAny<Guid>(),
                    null),
                Times.Once);
        }

        [Fact]
        public async Task GetById_ShouldReturnOk_WhenTaskExists()
        {
            var taskId = Guid.NewGuid();

            var taskDto = new TaskResponseDto
            {
                Id = taskId,
                Title = "Study"
            };

            _taskServiceMock
                .Setup(s => s.GetByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Guid>()))
                .ReturnsAsync(taskDto);

            var result = await _controller.GetById(taskId);

            result.Should().BeOfType<OkObjectResult>();

            var okResult = result as OkObjectResult;

            okResult!.Value.Should().Be(taskDto);

            _taskServiceMock.Verify(
                s => s.GetByIdAsync(
                    taskId,
                    It.IsAny<Guid>()),
                Times.Once);
        }

        [Fact]
        public async Task Create_ShouldReturnCreatedAtAction_WhenTaskIsCreated()
        {
            var dto = new CreateTaskDto
            {
                Title = "Study",
                Description = "Clean Architecture",
                DueDate = DateTime.UtcNow.AddDays(1)
            };

            var responseDto = new TaskResponseDto
            {
                Id = Guid.NewGuid(),
                Title = dto.Title
            };

            _taskServiceMock
                .Setup(s => s.CreateAsync(
                    dto,
                    It.IsAny<Guid>()))
                .ReturnsAsync(responseDto);

            var result = await _controller.Create(dto);

            result.Should().BeOfType<CreatedAtActionResult>();

            var createdResult = result as CreatedAtActionResult;

            createdResult!.ActionName.Should().Be(nameof(TasksController.GetById));

            createdResult.Value.Should().Be(responseDto);

            _taskServiceMock.Verify(
                s => s.CreateAsync(
                    dto,
                    It.IsAny<Guid>()),
                Times.Once);
        }

        [Fact]
        public async Task Update_ShouldReturnNoContent_WhenTaskIsUpdated()
        {
            var taskId = Guid.NewGuid();

            var dto = new UpdateTaskDto
            {
                Title = "Updated"
            };

            var result = await _controller.Update(taskId, dto);

            result.Should().BeOfType<NoContentResult>();

            _taskServiceMock.Verify(
                s => s.UpdateAsync(
                    taskId,
                    dto,
                    It.IsAny<Guid>()),
                Times.Once);
        }

        [Fact]
        public async Task Delete_ShouldReturnNoContent_WhenTaskIsDeleted()
        {
            var taskId = Guid.NewGuid();

            var result = await _controller.Delete(taskId);

            result.Should().BeOfType<NoContentResult>();

            _taskServiceMock.Verify(
                s => s.DeleteAsync(
                    taskId,
                    It.IsAny<Guid>()),
                Times.Once);
        }
    }
}