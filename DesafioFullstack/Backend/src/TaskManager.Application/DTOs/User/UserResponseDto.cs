namespace TaskManager.Application.DTOs.User
{
    public record UserResponseDto
    {
        public Guid Id { get; init; }
        public string Email { get; init; } = null!;
    }
}