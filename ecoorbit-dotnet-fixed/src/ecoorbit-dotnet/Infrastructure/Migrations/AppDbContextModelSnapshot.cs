using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using ecoorbit_dotnet.Infrastructure.Data;

#nullable disable

namespace ecoorbit_dotnet.Infrastructure.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "10.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            modelBuilder.Entity("ecoorbit_dotnet.Domain.Entities.FireDetectionResult", b =>
            {
                b.Property<Guid>("Id").HasColumnName("ID").HasColumnType("uniqueidentifier");
                b.Property<bool>("FireDetected").HasColumnName("FIRE_DETECTED").HasColumnType("bit");
                b.Property<int>("RiskLevel").HasColumnName("RISK_LEVEL").HasColumnType("int");
                b.Property<double>("ConfidenceScore").HasColumnName("CONFIDENCE_SCORE").HasColumnType("float");
                b.Property<string>("Notes").HasMaxLength(1000).HasColumnName("NOTES").HasColumnType("nvarchar(1000)");
                b.Property<DateTime>("AnalyzedAt").HasColumnName("ANALYZED_AT").HasColumnType("datetime2");
                b.Property<Guid>("SatelliteImageId").HasColumnName("SATELLITE_IMAGE_ID").HasColumnType("uniqueidentifier");
                b.HasKey("Id");
                b.HasIndex("SatelliteImageId").IsUnique();
                b.ToTable("FIRE_DETECTION_RESULTS");
            });

            modelBuilder.Entity("ecoorbit_dotnet.Domain.Entities.SatelliteImage", b =>
            {
                b.Property<Guid>("Id").HasColumnName("ID").HasColumnType("uniqueidentifier");
                b.Property<string>("ImageUrl").IsRequired().HasMaxLength(500).HasColumnName("IMAGE_URL").HasColumnType("nvarchar(500)");
                b.Property<string>("Region").IsRequired().HasMaxLength(200).HasColumnName("REGION").HasColumnType("nvarchar(200)");
                b.Property<double>("Latitude").HasColumnName("LATITUDE").HasColumnType("float");
                b.Property<double>("Longitude").HasColumnName("LONGITUDE").HasColumnType("float");
                b.Property<DateTime>("CapturedAt").HasColumnName("CAPTURED_AT").HasColumnType("datetime2");
                b.Property<DateTime>("SubmittedAt").HasColumnName("SUBMITTED_AT").HasColumnType("datetime2");
                b.Property<Guid>("UserId").HasColumnName("USER_ID").HasColumnType("uniqueidentifier");
                b.HasKey("Id");
                b.HasIndex("UserId");
                b.ToTable("SATELLITE_IMAGES");
            });

            modelBuilder.Entity("ecoorbit_dotnet.Domain.Entities.User", b =>
            {
                b.Property<Guid>("Id").HasColumnName("ID").HasColumnType("uniqueidentifier");
                b.Property<string>("Name").IsRequired().HasMaxLength(100).HasColumnName("NAME").HasColumnType("nvarchar(100)");
                b.Property<string>("Email").IsRequired().HasMaxLength(150).HasColumnName("EMAIL").HasColumnType("nvarchar(150)");
                b.Property<string>("PasswordHash").IsRequired().HasColumnName("PASSWORD_HASH").HasColumnType("nvarchar(max)");
                b.Property<string>("Role").IsRequired().HasMaxLength(50).HasColumnName("ROLE").HasColumnType("nvarchar(50)");
                b.Property<DateTime>("CreatedAt").HasColumnName("CREATED_AT").HasColumnType("datetime2");
                b.HasKey("Id");
                b.HasIndex("Email").IsUnique();
                b.ToTable("USERS");
            });
#pragma warning restore 612, 618
        }
    }
}
