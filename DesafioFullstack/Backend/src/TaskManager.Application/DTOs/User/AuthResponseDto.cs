namespace TaskManager.Application.DTOs.User
{
    public record AuthResponseDto
    {
        public string Token { get; init; } = null!;
        public UserResponseDto User { get; init; } = null!;
    }
}