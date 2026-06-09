using ecoorbit_dotnet.Application.DTOs.Flask;

namespace ecoorbit_dotnet.Application.Interfaces;

public interface IFlaskAnalysisClient
{
    Task<FlaskAnalysisResponseDto?> AnalyzeAsync(double lat, double lon, DateTime capturedAt);
}