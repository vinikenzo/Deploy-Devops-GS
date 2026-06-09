using ecoorbit_dotnet.Application.DTOs.Auth;
using ecoorbit_dotnet.Application.Services;
using ecoorbit_dotnet.Domain.Entities;
using ecoorbit_dotnet.Infrastructure.Repositories.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;

namespace ecoorbit_dotnet.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly IConfiguration _configuration;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _userRepoMock = new Mock<IUserRepository>();

        var inMemorySettings = new Dictionary<string, string>
        {
            { "Jwt:Key", "test-super-secret-key-min-32-characters" },
            { "Jwt:Issuer", "FireDetectionApi" },
            { "Jwt:Audience", "FireDetectionApiUsers" }
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();

        _authService = new AuthService(_userRepoMock.Object, _configuration);
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnToken_WhenEmailIsNew()
    {
        // Arrange
        var dto = new RegisterRequestDto { Name = "Test User", Email = "test@test.com", Password = "password123" };
        _userRepoMock.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync((User?)null);
        _userRepoMock.Setup(r => r.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

        // Act
        var result = await _authService.RegisterAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().NotBeNullOrEmpty();
        result.Email.Should().Be(dto.Email);
    }

    [Fact]
    public async Task RegisterAsync_ShouldThrow_WhenEmailAlreadyExists()
    {
        // Arrange
        var dto = new RegisterRequestDto { Name = "Test User", Email = "existing@test.com", Password = "password123" };
        var existingUser = new User { Id = Guid.NewGuid(), Email = dto.Email };
        _userRepoMock.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync(existingUser);

        // Act
        var act = async () => await _authService.RegisterAsync(dto);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Email already in use.");
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnToken_WhenCredentialsAreValid()
    {
        // Arrange
        var dto = new LoginRequestDto { Email = "test@test.com", Password = "password123" };
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = "Analyst"
        };
        _userRepoMock.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync(user);

        // Act
        var result = await _authService.LoginAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task LoginAsync_ShouldThrow_WhenPasswordIsWrong()
    {
        // Arrange
        var dto = new LoginRequestDto { Email = "test@test.com", Password = "wrongpassword" };
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("correctpassword"),
            Role = "Analyst"
        };
        _userRepoMock.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync(user);

        // Act
        var act = async () => await _authService.LoginAsync(dto);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }
}