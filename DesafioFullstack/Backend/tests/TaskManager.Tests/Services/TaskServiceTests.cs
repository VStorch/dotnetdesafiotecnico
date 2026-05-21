using AutoMapper;
using FluentAssertions;
using Moq;
using TaskManager.Application.DTOs.Tasks;
using TaskManager.Application.Services;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Exceptions;
using TaskManager.Domain.Repositories;

namespace TaskManager.Tests.Services
{
    public class TaskServiceTests
    {
        private readonly Mock<ITaskRepository> _taskRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly TaskService _taskService;

        public TaskServiceTests()
        {
            _taskRepositoryMock = new Mock<ITaskRepository>();
            _mapperMock = new Mock<IMapper>();

            _taskService = new TaskService(
                _taskRepositoryMock.Object,
                _mapperMock.Object
            );
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllUserTasks()
        {
            var userId = Guid.NewGuid();

            var tasks = new List<TaskItem>
            {
                new("Task 1", DateTime.UtcNow, userId),
                new("Task 2", DateTime.UtcNow, userId)
            };

            var responseDtos = new List<TaskResponseDto>
            {
                new() { Title = "Task 1" },
                new() { Title = "Task 2" }
            };

            _taskRepositoryMock
                .Setup(r => r.GetByUserIdAsync(userId, null))
                .ReturnsAsync(tasks);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<TaskResponseDto>>(tasks))
                .Returns(responseDtos);

            var result = await _taskService.GetAllAsync(userId);

            result.Should().HaveCount(2);

            _taskRepositoryMock.Verify(
                r => r.GetByUserIdAsync(userId, null),
                Times.Once);

            _mapperMock.Verify(
                m => m.Map<IEnumerable<TaskResponseDto>>(tasks),
                Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_ShouldFilterByIsCompleted_WhenProvided()
        {
            var userId = Guid.NewGuid();
            var tasks = new List<TaskItem> { new("Task 1", DateTime.UtcNow, userId) };

            _taskRepositoryMock
                .Setup(r => r.GetByUserIdAsync(userId, true))
                .ReturnsAsync(tasks);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<TaskResponseDto>>(tasks))
                .Returns(new List<TaskResponseDto> { new() { Title = "Task 1" } });

            var result = await _taskService.GetAllAsync(userId, isCompleted: true);

            result.Should().HaveCount(1);

            _taskRepositoryMock.Verify(
                r => r.GetByUserIdAsync(userId, true),
                Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnTask_WhenTaskExists()
        {
            var userId = Guid.NewGuid();
            var taskId = Guid.NewGuid();

            var task = new TaskItem("Study", DateTime.UtcNow, userId);

            var responseDto = new TaskResponseDto
            {
                Id = taskId,
                Title = "Study"
            };

            _taskRepositoryMock
                .Setup(r => r.GetByIdAndUserIdAsync(taskId, userId))
                .ReturnsAsync(task);

            _mapperMock
                .Setup(m => m.Map<TaskResponseDto>(task))
                .Returns(responseDto);

            var result = await _taskService.GetByIdAsync(taskId, userId);

            result.Should().NotBeNull();
            result.Title.Should().Be("Study");

            _taskRepositoryMock.Verify(
                r => r.GetByIdAndUserIdAsync(taskId, userId),
                Times.Once);

            _mapperMock.Verify(
                m => m.Map<TaskResponseDto>(task),
                Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrowTaskNotFoundException_WhenTaskDoesNotExist()
        {
            var taskId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _taskRepositoryMock
                .Setup(r => r.GetByIdAndUserIdAsync(taskId, userId))
                .ReturnsAsync((TaskItem?)null);

            Func<Task> act = async () =>
                await _taskService.GetByIdAsync(taskId, userId);

            await act.Should()
                .ThrowAsync<TaskNotFoundException>();

            _mapperMock.Verify(
                m => m.Map<TaskResponseDto>(It.IsAny<TaskItem>()),
                Times.Never);
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateTaskSuccessfully()
        {
            var userId = Guid.NewGuid();

            var dto = new CreateTaskDto
            {
                Title = "Study",
                Description = "Clean Architecture",
                DueDate = DateTime.UtcNow.AddDays(1)
            };

            var responseDto = new TaskResponseDto
            {
                Title = dto.Title,
                Description = dto.Description
            };

            _mapperMock
                .Setup(m => m.Map<TaskResponseDto>(It.IsAny<TaskItem>()))
                .Returns(responseDto);

            var result = await _taskService.CreateAsync(dto, userId);

            result.Should().NotBeNull();
            result.Title.Should().Be(dto.Title);

            _taskRepositoryMock.Verify(
                r => r.AddAsync(It.Is<TaskItem>(t =>
                    t.Title == dto.Title &&
                    t.Description == dto.Description &&
                    t.UserId == userId)),
                Times.Once);

            _taskRepositoryMock.Verify(
                r => r.SaveChangesAsync(),
                Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateTask_WhenTaskExists()
        {
            var taskId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var task = new TaskItem(
                "Old Title",
                DateTime.UtcNow,
                userId);

            var dto = new UpdateTaskDto
            {
                Title = "New Title",
                Description = "Updated Description",
                DueDate = DateTime.UtcNow.AddDays(2),
                IsCompleted = true
            };

            _taskRepositoryMock
                .Setup(r => r.GetByIdAndUserIdAsync(taskId, userId))
                .ReturnsAsync(task);

            await _taskService.UpdateAsync(taskId, dto, userId);

            _mapperMock.Verify(
                m => m.Map(dto, task),
                Times.Once);

            _taskRepositoryMock.Verify(
                r => r.Update(task),
                Times.Once);

            _taskRepositoryMock.Verify(
                r => r.SaveChangesAsync(),
                Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowTaskNotFoundException_WhenTaskDoesNotExist()
        {
            var taskId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var dto = new UpdateTaskDto
            {
                Title = "Updated"
            };

            _taskRepositoryMock
                .Setup(r => r.GetByIdAndUserIdAsync(taskId, userId))
                .ReturnsAsync((TaskItem?)null);

            Func<Task> act = async () =>
                await _taskService.UpdateAsync(taskId, dto, userId);

            await act.Should()
                .ThrowAsync<TaskNotFoundException>();

            _mapperMock.Verify(
                m => m.Map(It.IsAny<UpdateTaskDto>(), It.IsAny<TaskItem>()),
                Times.Never);

            _taskRepositoryMock.Verify(
                r => r.Update(It.IsAny<TaskItem>()),
                Times.Never);

            _taskRepositoryMock.Verify(
                r => r.SaveChangesAsync(),
                Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteTask_WhenTaskExists()
        {
            var taskId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var task = new TaskItem(
                "Study",
                DateTime.UtcNow,
                userId);

            _taskRepositoryMock
                .Setup(r => r.GetByIdAndUserIdAsync(taskId, userId))
                .ReturnsAsync(task);

            await _taskService.DeleteAsync(taskId, userId);

            _taskRepositoryMock.Verify(
                r => r.Delete(task),
                Times.Once);

            _taskRepositoryMock.Verify(
                r => r.SaveChangesAsync(),
                Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowTaskNotFoundException_WhenTaskDoesNotExist()
        {
            var taskId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _taskRepositoryMock
                .Setup(r => r.GetByIdAndUserIdAsync(taskId, userId))
                .ReturnsAsync((TaskItem?)null);

            Func<Task> act = async () =>
                await _taskService.DeleteAsync(taskId, userId);

            await act.Should()
                .ThrowAsync<TaskNotFoundException>();

            _taskRepositoryMock.Verify(
                r => r.Delete(It.IsAny<TaskItem>()),
                Times.Never);

            _taskRepositoryMock.Verify(
                r => r.SaveChangesAsync(),
                Times.Never);
        }
    }
}