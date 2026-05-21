namespace TaskManager.Application.DTOs
{
    public record LoginUserDto
    {
        public string Email { get; init; } = null!;
        public string Password { get; init; } = null!;
    }
}