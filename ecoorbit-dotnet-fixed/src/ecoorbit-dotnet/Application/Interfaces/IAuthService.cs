using ecoorbit_dotnet.Application.DTOs.Auth;

namespace ecoorbit_dotnet.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto> RegisterAsync(RegisterRequestDto dto);
    Task<LoginResponseDto> LoginAsync(LoginRequestDto dto);
}