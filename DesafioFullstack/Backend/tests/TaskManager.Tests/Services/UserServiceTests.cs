using AutoMapper;
using FluentAssertions;
using Moq;
using TaskManager.Application.DTOs.User;
using TaskManager.Application.Interfaces.Security;
using TaskManager.Application.Services;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Exceptions;
using TaskManager.Domain.Repositories;

namespace TaskManager.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ITokenService> _tokenServiceMock;

        private readonly UserService _userService;

        public UserServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _mapperMock = new Mock<IMapper>();
            _tokenServiceMock = new Mock<ITokenService>();

            _userService = new UserService(
                _userRepositoryMock.Object,
                _passwordHasherMock.Object,
                _mapperMock.Object,
                _tokenServiceMock.Object
            );
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnAuthResponse_WhenCredentialsAreValid()
        {
            var loginDto = new LoginUserDto
            {
                Email = "user@test.com",
                Password = "123456"
            };

            var user = new User
            {
                Email = loginDto.Email,
                PasswordHash = "hashed-password"
            };

            var userResponseDto = new UserResponseDto
            {
                Email = user.Email
            };

            var token = "jwt-token";

            _userRepositoryMock
                .Setup(r => r.GetByEmailAsync(loginDto.Email))
                .ReturnsAsync(user);

            _passwordHasherMock
                .Setup(h => h.VerifyPassword(loginDto.Password, user.PasswordHash))
                .Returns(true);

            _tokenServiceMock
                .Setup(t => t.GenerateToken(user))
                .Returns(token);

            _mapperMock
                .Setup(m => m.Map<UserResponseDto>(user))
                .Returns(userResponseDto);

            var result = await _userService.LoginAsync(loginDto);

            result.Should().NotBeNull();
            result.Token.Should().Be(token);
            result.User.Email.Should().Be(user.Email);

            _userRepositoryMock.Verify(
                r => r.GetByEmailAsync(loginDto.Email),
                Times.Once);

            _passwordHasherMock.Verify(
                h => h.VerifyPassword(loginDto.Password, user.PasswordHash),
                Times.Once);

            _tokenServiceMock.Verify(
                t => t.GenerateToken(user),
                Times.Once);
        }

        [Fact]
        public async Task LoginAsync_ShouldThrowInvalidCredentialsException_WhenUserDoesNotExist()
        {
            var loginDto = new LoginUserDto
            {
                Email = "user@test.com",
                Password = "123456"
            };

            _userRepositoryMock
                .Setup(r => r.GetByEmailAsync(loginDto.Email))
                .ReturnsAsync((User?)null);

            Func<Task> act = async () =>
                await _userService.LoginAsync(loginDto);

            await act.Should()
                .ThrowAsync<InvalidCredentialsException>();

            _tokenServiceMock.Verify(
                t => t.GenerateToken(It.IsAny<User>()),
                Times.Never);
        }

        [Fact]
        public async Task LoginAsync_ShouldThrowInvalidCredentialsException_WhenPasswordIsInvalid()
        {
            var loginDto = new LoginUserDto
            {
                Email = "user@test.com",
                Password = "wrong-password"
            };

            var user = new User
            {
                Email = loginDto.Email,
                PasswordHash = "hashed-password"
            };

            _userRepositoryMock
                .Setup(r => r.GetByEmailAsync(loginDto.Email))
                .ReturnsAsync(user);

            _passwordHasherMock
                .Setup(h => h.VerifyPassword(loginDto.Password, user.PasswordHash))
                .Returns(false);

            Func<Task> act = async () =>
                await _userService.LoginAsync(loginDto);

            await act.Should()
                .ThrowAsync<InvalidCredentialsException>();

            _tokenServiceMock.Verify(
                t => t.GenerateToken(It.IsAny<User>()),
                Times.Never);
        }

        [Fact]
        public async Task RegisterAsync_ShouldRegisterUser_WhenEmailIsUnique()
        {
            var registerDto = new RegisterUserDto
            {
                Email = "user@test.com",
                Password = "123456"
            };

            var user = new User
            {
                Email = registerDto.Email
            };

            var userResponseDto = new UserResponseDto
            {
                Email = user.Email
            };

            _userRepositoryMock
                .Setup(r => r.GetByEmailAsync(registerDto.Email))
                .ReturnsAsync((User?)null);

            _mapperMock
                .Setup(m => m.Map<User>(registerDto))
                .Returns(user);

            _passwordHasherMock
                .Setup(h => h.HashPassword(registerDto.Password))
                .Returns("hashed-password");

            _mapperMock
                .Setup(m => m.Map<UserResponseDto>(user))
                .Returns(userResponseDto);

            var result = await _userService.RegisterAsync(registerDto);

            result.Should().NotBeNull();
            result.Email.Should().Be(registerDto.Email);

            user.PasswordHash.Should().Be("hashed-password");

            _userRepositoryMock.Verify(
                r => r.AddAsync(user),
                Times.Once);

            _passwordHasherMock.Verify(
                h => h.HashPassword(registerDto.Password),
                Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_ShouldThrowEmailAlreadyExistsException_WhenEmailAlreadyExists()
        {
            var registerDto = new RegisterUserDto
            {
                Email = "user@test.com"
            };

            var existingUser = new User
            {
                Email = registerDto.Email
            };

            _userRepositoryMock
                .Setup(r => r.GetByEmailAsync(registerDto.Email))
                .ReturnsAsync(existingUser);

            Func<Task> act = async () =>
                await _userService.RegisterAsync(registerDto);

            await act.Should()
                .ThrowAsync<EmailAlreadyExistsException>();

            _userRepositoryMock.Verify(
                r => r.AddAsync(It.IsAny<User>()),
                Times.Never);

            _passwordHasherMock.Verify(
                h => h.HashPassword(It.IsAny<string>()),
                Times.Never);
        }
    }
}