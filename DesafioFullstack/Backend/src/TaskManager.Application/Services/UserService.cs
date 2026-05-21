using AutoMapper;
using TaskManager.Application.DTOs;
using TaskManager.Application.Interfaces.Security;
using TaskManager.Application.Interfaces.Services;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Exceptions;
using TaskManager.Domain.Repositories;

namespace TaskManager.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher, IMapper mapper)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _mapper = mapper;
        }

        public async Task<UserResponseDto> RegisterAsync(RegisterUserDto registerDto)
        {
            await ValidateEmailUniqueness(registerDto.Email);

            var user = _mapper.Map<User>(registerDto);

            user.PasswordHash = _passwordHasher.HashPassword(registerDto.Password);

            await _userRepository.AddAsync(user);

            return _mapper.Map<UserResponseDto>(user);
        }

        private async Task ValidateEmailUniqueness(string email)
        {
            var existingUser = await _userRepository.GetByEmailAsync(email);
            if (existingUser != null)
            {
                throw new EmailAlreadyExistsException(email);
            }
        }
    }
}