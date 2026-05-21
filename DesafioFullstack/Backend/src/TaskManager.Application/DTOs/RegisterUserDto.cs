namespace TaskManager.Application.DTOs
{
    public record RegisterUserDto
    {
        public string Email { get; init; } = null!;
        public string Password { get; init; } = null!;
    }
}