using ecoorbit_dotnet.Application.DTOs.FireDetectionResult;
using ecoorbit_dotnet.Application.Services;
using ecoorbit_dotnet.Domain.Entities;
using ecoorbit_dotnet.Domain.Enums;
using ecoorbit_dotnet.Infrastructure.Repositories.Interfaces;
using FluentAssertions;
using Moq;

namespace ecoorbit_dotnet.Tests.Services;

public class FireDetectionResultServiceTests
{
    private readonly Mock<IFireDetectionResultRepository> _resultRepoMock;
    private readonly Mock<ISatelliteImageRepository> _imageRepoMock;
    private readonly FireDetectionResultService _service;

    public FireDetectionResultServiceTests()
    {
        _resultRepoMock = new Mock<IFireDetectionResultRepository>();
        _imageRepoMock = new Mock<ISatelliteImageRepository>();
        _service = new FireDetectionResultService(_resultRepoMock.Object, _imageRepoMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnDto_WhenImageExistsAndNoResultYet()
    {
        // Arrange
        var imageId = Guid.NewGuid();
        var image = new SatelliteImage { Id = imageId, Region = "Pantanal" };
        var dto = new CreateFireDetectionResultDto
        {
            SatelliteImageId = imageId,
            FireDetected = true,
            RiskLevel = FireRiskLevel.High,
            ConfidenceScore = 0.92,
            Notes = "Active fire detected"
        };

        _imageRepoMock.Setup(r => r.GetByIdAsync(imageId)).ReturnsAsync(image);
        _resultRepoMock.Setup(r => r.GetBySatelliteImageIdAsync(imageId)).ReturnsAsync((FireDetectionResult?)null);
        _resultRepoMock.Setup(r => r.AddAsync(It.IsAny<FireDetectionResult>())).Returns(Task.CompletedTask);

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.FireDetected.Should().BeTrue();
        result.RiskLevel.Should().Be(FireRiskLevel.High);
        result.ConfidenceScore.Should().Be(0.92);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenResultAlreadyExists()
    {
        // Arrange
        var imageId = Guid.NewGuid();
        var image = new SatelliteImage { Id = imageId };
        var existing = new FireDetectionResult { Id = Guid.NewGuid(), SatelliteImageId = imageId };
        var dto = new CreateFireDetectionResultDto { SatelliteImageId = imageId };

        _imageRepoMock.Setup(r => r.GetByIdAsync(imageId)).ReturnsAsync(image);
        _resultRepoMock.Setup(r => r.GetBySatelliteImageIdAsync(imageId)).ReturnsAsync(existing);

        // Act
        var act = async () => await _service.CreateAsync(dto);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("A detection result already exists for this image.");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrow_WhenResultNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        _resultRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((FireDetectionResult?)null);

        // Act
        var act = async () => await _service.GetByIdAsync(id);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}