namespace ecoorbit_dotnet.Application.DTOs.SatelliteImage;

public class SatelliteImageResponseDto
{
    public Guid Id { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime CapturedAt { get; set; }
    public DateTime SubmittedAt { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public bool HasDetectionResult { get; set; }
}