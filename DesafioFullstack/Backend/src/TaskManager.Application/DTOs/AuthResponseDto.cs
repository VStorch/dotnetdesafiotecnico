namespace TaskManager.Application.DTOs
{
    public record AuthResponseDto
    {
        public string Token { get; init; } = null!;
        public UserResponseDto User { get; init; } = null!;
    }
}