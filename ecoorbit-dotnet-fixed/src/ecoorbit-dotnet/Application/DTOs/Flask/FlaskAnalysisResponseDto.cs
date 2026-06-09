using System.Text.Json.Serialization;

namespace ecoorbit_dotnet.Application.DTOs.Flask;

public class FlaskAnalysisResponseDto
{
    [JsonPropertyName("fogo_detectado")]
    public bool FireDetected { get; set; }

    [JsonPropertyName("confianca_percentual")]
    public double ConfidencePercentage { get; set; }

    [JsonPropertyName("arquivo_analisado")]
    public string ArquivoAnalisado { get; set; } = string.Empty;
}