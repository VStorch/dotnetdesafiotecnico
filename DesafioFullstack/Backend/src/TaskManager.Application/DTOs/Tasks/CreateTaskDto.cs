namespace TaskManager.Application.DTOs.Tasks
{
    public record CreateTaskDto
    {
        public string Title { get; init; } = null!;
        public string? Description { get; init; }
        public DateTime DueDate { get; init; }
    }
}