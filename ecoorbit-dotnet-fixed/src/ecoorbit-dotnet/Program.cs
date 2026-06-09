using System.Text;
using ecoorbit_dotnet.Api.Middleware;
using ecoorbit_dotnet.Application.Interfaces;
using ecoorbit_dotnet.Application.Services;
using ecoorbit_dotnet.Infrastructure.Data;
using ecoorbit_dotnet.Infrastructure.Http;
using ecoorbit_dotnet.Infrastructure.Repositories.Implementations;
using ecoorbit_dotnet.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ISatelliteImageRepository, SatelliteImageRepository>();
builder.Services.AddScoped<IFireDetectionResultRepository, FireDetectionResultRepository>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ISatelliteImageService, SatelliteImageService>();
builder.Services.AddScoped<IFireDetectionResultService, FireDetectionResultService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddHttpClient<IFlaskAnalysisClient, FlaskAnalysisClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["FlaskApi:BaseUrl"]!);
    client.Timeout = TimeSpan.FromSeconds(60);
});

builder.Services.AddHttpClient("nasa", client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddControllers();

builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>("database");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "EcoOrbit Fire Detection API",
        Version = "v1",
        Description = "API for satellite-based wildfire detection - Azure SQL Server"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "EcoOrbit API v1");
    c.RoutePrefix = string.Empty;
});

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

public partial class Program { }
