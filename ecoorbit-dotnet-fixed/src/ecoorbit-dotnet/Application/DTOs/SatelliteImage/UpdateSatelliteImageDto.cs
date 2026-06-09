using System.ComponentModel.DataAnnotations;

namespace ecoorbit_dotnet.Application.DTOs.SatelliteImage;

public class UpdateSatelliteImageDto
{
    [Required]
    [MaxLength(200)]
    public string Region { get; set; } = string.Empty;

    [Range(-90, 90)]
    public double Latitude { get; set; }

    [Range(-180, 180)]
    public double Longitude { get; set; }

    [Required]
    public DateTime CapturedAt { get; set; }
}
