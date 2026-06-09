using System.Text.Json;
using ecoorbit_dotnet.Application.DTOs.Flask;
using ecoorbit_dotnet.Application.Interfaces;

namespace ecoorbit_dotnet.Infrastructure.Http;

public class FlaskAnalysisClient : IFlaskAnalysisClient
{
    private readonly HttpClient _httpClient;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<FlaskAnalysisClient> _logger;

    public FlaskAnalysisClient(HttpClient httpClient, IHttpClientFactory httpClientFactory, ILogger<FlaskAnalysisClient> logger)
    {
        _httpClient = httpClient;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<FlaskAnalysisResponseDto?> AnalyzeAsync(double lat, double lon, DateTime capturedAt)
    {
        try
        {
            var imageBytes = await DownloadNasaImageAsync(lat, lon, capturedAt);
            if (imageBytes is null)
            {
                _logger.LogWarning("Failed to download NASA image for lat={Lat}, lon={Lon}", lat, lon);
                return null;
            }

            using var form = new MultipartFormDataContent();
            using var imageContent = new ByteArrayContent(imageBytes);
            imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
            form.Add(imageContent, "imagem", "satellite.jpg");

            var response = await _httpClient.PostAsync("/api/v1/predict", form);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Flask response: {Json}", json);

            return JsonSerializer.Deserialize<FlaskAnalysisResponseDto>(json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Flask analysis failed for lat={Lat}, lon={Lon}", lat, lon);
            return null;
        }
    }

    private async Task<byte[]?> DownloadNasaImageAsync(double lat, double lon, DateTime date)
    {
        try
        {
            var delta = 1.0;
            var south = lat - delta;
            var north = lat + delta;
            var west = lon - delta;
            var east = lon + delta;
            var dateStr = date.ToString("yyyy-MM-dd");

            var nasaUrl = $"https://wvs.earthdata.nasa.gov/api/v1/snapshot" +
                          $"?REQUEST=GetSnapshot" +
                          $"&TIME={dateStr}" +
                          $"&BBOX={south},{west},{north},{east}" +
                          $"&CRS=EPSG:4326" +
                          $"&LAYERS=MODIS_Terra_CorrectedReflectance_TrueColor" +
                          $"&WRAP=DAY" +
                          $"&WIDTH=512&HEIGHT=512" +
                          $"&FORMAT=image/jpeg";

            var nasaClient = _httpClientFactory.CreateClient("nasa");
            return await nasaClient.GetByteArrayAsync(nasaUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "NASA image download failed");
            return null;
        }
    }
}