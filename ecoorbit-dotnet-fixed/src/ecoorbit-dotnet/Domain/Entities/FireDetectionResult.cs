using ecoorbit_dotnet.Domain.Enums;

namespace ecoorbit_dotnet.Domain.Entities;

public class FireDetectionResult
{
    public Guid Id { get; set; }
    public bool FireDetected { get; set; }
    public FireRiskLevel RiskLevel { get; set; }
    public double ConfidenceScore { get; set; }
    public string Notes { get; set; } = string.Empty;
    public DateTime AnalyzedAt { get; set; } = DateTime.UtcNow;

    public Guid SatelliteImageId { get; set; }
    public SatelliteImage SatelliteImage { get; set; } = null!;
}