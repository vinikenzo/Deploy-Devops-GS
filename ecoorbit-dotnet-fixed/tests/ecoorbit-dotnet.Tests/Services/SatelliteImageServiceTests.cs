using ecoorbit_dotnet.Application.DTOs.SatelliteImage;
using ecoorbit_dotnet.Application.Interfaces;
using ecoorbit_dotnet.Application.Services;
using ecoorbit_dotnet.Domain.Entities;
using ecoorbit_dotnet.Infrastructure.Repositories.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace ecoorbit_dotnet.Tests.Services;

public class SatelliteImageServiceTests
{
    private readonly Mock<ISatelliteImageRepository> _imageRepoMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IFireDetectionResultRepository> _resultRepoMock;
    private readonly Mock<IFlaskAnalysisClient> _flaskClientMock;
    private readonly Mock<ILogger<SatelliteImageService>> _loggerMock;
    private readonly Mock<IServiceScopeFactory> _scopeFactoryMock;
    private readonly SatelliteImageService _service;

    public SatelliteImageServiceTests()
    {
        _imageRepoMock = new Mock<ISatelliteImageRepository>();
        _userRepoMock = new Mock<IUserRepository>();
        _resultRepoMock = new Mock<IFireDetectionResultRepository>();
        _flaskClientMock = new Mock<IFlaskAnalysisClient>();
        _loggerMock = new Mock<ILogger<SatelliteImageService>>();
        _scopeFactoryMock = new Mock<IServiceScopeFactory>();

        var scopeMock = new Mock<IServiceScope>();
        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock.Setup(p => p.GetService(typeof(IFireDetectionResultRepository)))
            .Returns(_resultRepoMock.Object);
        scopeMock.Setup(s => s.ServiceProvider).Returns(serviceProviderMock.Object);
        _scopeFactoryMock.Setup(f => f.CreateScope()).Returns(scopeMock.Object);

        _service = new SatelliteImageService(
            _imageRepoMock.Object,
            _userRepoMock.Object,
            _resultRepoMock.Object,
            _flaskClientMock.Object,
            _loggerMock.Object,
            _scopeFactoryMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnDto_WhenImageExists()
    {
        // Arrange
        var imageId = Guid.NewGuid();
        var image = new SatelliteImage
        {
            Id = imageId,
            ImageUrl = "http://example.com/img.png",
            Region = "Amazônia",
            Latitude = -3.0,
            Longitude = -60.0,
            CapturedAt = DateTime.UtcNow,
            UserId = Guid.NewGuid(),
            User = new User { Name = "Analyst" }
        };
        _imageRepoMock.Setup(r => r.GetByIdAsync(imageId)).ReturnsAsync(image);

        // Act
        var result = await _service.GetByIdAsync(imageId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(imageId);
        result.Region.Should().Be("Amazônia");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrow_WhenImageNotFound()
    {
        // Arrange
        var imageId = Guid.NewGuid();
        _imageRepoMock.Setup(r => r.GetByIdAsync(imageId)).ReturnsAsync((SatelliteImage?)null);

        // Act
        var act = async () => await _service.GetByIdAsync(imageId);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnDto_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, Name = "Analyst", Email = "a@a.com" };
        var dto = new CreateSatelliteImageDto
        {
            Region = "Cerrado",
            Latitude = -15.0,
            Longitude = -47.0,
            CapturedAt = DateTime.UtcNow
        };

        _userRepoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
        _imageRepoMock.Setup(r => r.AddAsync(It.IsAny<SatelliteImage>())).Returns(Task.CompletedTask);
        _flaskClientMock.Setup(f => f.AnalyzeAsync(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<DateTime>()))
            .ReturnsAsync((ecoorbit_dotnet.Application.DTOs.Flask.FlaskAnalysisResponseDto?)null);

        // Act
        var result = await _service.CreateAsync(dto, userId);

        // Assert
        result.Should().NotBeNull();
        result.Region.Should().Be("Cerrado");
        result.UserId.Should().Be(userId);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenUserNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dto = new CreateSatelliteImageDto
        {
            Region = "Cerrado",
            Latitude = -15.0,
            Longitude = -47.0,
            CapturedAt = DateTime.UtcNow
        };

        _userRepoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((User?)null);

        // Act
        var act = async () => await _service.CreateAsync(dto, userId);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}