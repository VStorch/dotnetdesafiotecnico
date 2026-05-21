using TaskManager.Application.DTOs;

namespace TaskManager.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<UserResponseDto> RegisterAsync(RegisterUserDto registerDto);

        Task<AuthResponseDto> LoginAsync(LoginUserDto loginDto);
    }
}