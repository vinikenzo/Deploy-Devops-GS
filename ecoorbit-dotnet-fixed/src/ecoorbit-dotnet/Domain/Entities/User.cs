namespace ecoorbit_dotnet.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "Analyst";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<SatelliteImage> SatelliteImages { get; set; } = new List<SatelliteImage>();
}