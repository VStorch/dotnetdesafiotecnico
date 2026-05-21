namespace TaskManager.Application.DTOs.Tasks
{
    public record UpdateTaskDto
    {
        public string Title { get; init; } = null!;
        public string? Description { get; init; }
        public DateTime DueDate { get; init; }
        public bool IsCompleted { get; init; }
    }
}