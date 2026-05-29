using OdontoCare.Api.DTOs;

namespace OdontoCare.Api.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegistrarAsync(RegisterDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
}