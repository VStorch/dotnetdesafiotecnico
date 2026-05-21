namespace TaskManager.Application.DTOs.Tasks
{
    public record TaskResponseDto
    {
        public Guid Id { get; init; }
        public string Title { get; init; } = null!;
        public string? Description { get; init; }
        public DateTime DueDate { get; init; }
        public bool IsCompleted { get; init; }
    }
}