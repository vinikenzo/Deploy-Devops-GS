using ecoorbit_dotnet.Domain.Enums;

namespace ecoorbit_dotnet.Application.DTOs.FireDetectionResult;

public class FireDetectionResultResponseDto
{
    public Guid Id { get; set; }
    public bool FireDetected { get; set; }
    public FireRiskLevel RiskLevel { get; set; }
    public string RiskLevelName { get; set; } = string.Empty;
    public double ConfidenceScore { get; set; }
    public string Notes { get; set; } = string.Empty;
    public DateTime AnalyzedAt { get; set; }
    public Guid SatelliteImageId { get; set; }
    public string Region { get; set; } = string.Empty;
}