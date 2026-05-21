namespace TaskManager.Application.DTOs
{
    public record UserResponseDto
    {
        public Guid Id { get; init; }
        public string Email { get; init; } = null!;
    }
}