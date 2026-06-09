namespace ecoorbit_dotnet.Domain.Entities;

public class SatelliteImage
{
    public Guid Id { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime CapturedAt { get; set; }
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public FireDetectionResult? DetectionResult { get; set; }
}