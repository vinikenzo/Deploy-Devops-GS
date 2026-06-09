using System.ComponentModel.DataAnnotations;
using ecoorbit_dotnet.Domain.Enums;

namespace ecoorbit_dotnet.Application.DTOs.FireDetectionResult;

public class CreateFireDetectionResultDto
{
    [Required]
    public Guid SatelliteImageId { get; set; }

    public bool FireDetected { get; set; }

    [Required]
    public FireRiskLevel RiskLevel { get; set; }

    [Range(0, 1)]
    public double ConfidenceScore { get; set; }

    [MaxLength(1000)]
    public string Notes { get; set; } = string.Empty;
}