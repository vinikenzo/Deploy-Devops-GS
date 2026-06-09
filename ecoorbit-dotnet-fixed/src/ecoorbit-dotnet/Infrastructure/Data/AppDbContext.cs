using ecoorbit_dotnet.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ecoorbit_dotnet.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<SatelliteImage> SatelliteImages => Set<SatelliteImage>();
    public DbSet<FireDetectionResult> FireDetectionResults => Set<FireDetectionResult>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(e =>
        {
            e.ToTable("USERS");
            e.HasKey(u => u.Id);
            e.Property(u => u.Id).HasColumnName("ID");
            e.Property(u => u.Name).HasColumnName("NAME").HasMaxLength(100).IsRequired();
            e.Property(u => u.Email).HasColumnName("EMAIL").HasMaxLength(150).IsRequired();
            e.Property(u => u.PasswordHash).HasColumnName("PASSWORD_HASH").IsRequired();
            e.Property(u => u.Role).HasColumnName("ROLE").HasMaxLength(50);
            e.Property(u => u.CreatedAt).HasColumnName("CREATED_AT");
            e.HasIndex(u => u.Email).IsUnique();
        });

        modelBuilder.Entity<SatelliteImage>(e =>
        {
            e.ToTable("SATELLITE_IMAGES");
            e.HasKey(s => s.Id);
            e.Property(s => s.Id).HasColumnName("ID");
            e.Property(s => s.ImageUrl).HasColumnName("IMAGE_URL").HasMaxLength(500).IsRequired();
            e.Property(s => s.Region).HasColumnName("REGION").HasMaxLength(200).IsRequired();
            e.Property(s => s.Latitude).HasColumnName("LATITUDE");
            e.Property(s => s.Longitude).HasColumnName("LONGITUDE");
            e.Property(s => s.CapturedAt).HasColumnName("CAPTURED_AT");
            e.Property(s => s.SubmittedAt).HasColumnName("SUBMITTED_AT");
            e.Property(s => s.UserId).HasColumnName("USER_ID");
            e.HasOne(s => s.User)
             .WithMany(u => u.SatelliteImages)
             .HasForeignKey(s => s.UserId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<FireDetectionResult>(e =>
        {
            e.ToTable("FIRE_DETECTION_RESULTS");
            e.HasKey(f => f.Id);
            e.Property(f => f.Id).HasColumnName("ID");
            e.Property(f => f.FireDetected).HasColumnName("FIRE_DETECTED");
            e.Property(f => f.RiskLevel).HasColumnName("RISK_LEVEL").HasConversion<int>();
            e.Property(f => f.ConfidenceScore).HasColumnName("CONFIDENCE_SCORE");
            e.Property(f => f.Notes).HasColumnName("NOTES").HasMaxLength(1000);
            e.Property(f => f.AnalyzedAt).HasColumnName("ANALYZED_AT");
            e.Property(f => f.SatelliteImageId).HasColumnName("SATELLITE_IMAGE_ID");
            e.HasOne(f => f.SatelliteImage)
             .WithOne(s => s.DetectionResult)
             .HasForeignKey<FireDetectionResult>(f => f.SatelliteImageId)
             .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
