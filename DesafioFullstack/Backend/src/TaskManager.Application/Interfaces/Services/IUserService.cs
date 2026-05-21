
using TaskManager.Application.DTOs.User;

namespace TaskManager.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<UserResponseDto> RegisterAsync(RegisterUserDto registerDto);

        Task<AuthResponseDto> LoginAsync(LoginUserDto loginDto);
    }
}