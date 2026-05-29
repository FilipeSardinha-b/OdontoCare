using Microsoft.AspNetCore.Mvc;
using OdontoCare.Api.DTOs;
using OdontoCare.Api.Services;

namespace OdontoCare.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _service;

    public AuthController(IAuthService service)
    {
        _service = service;
    }

    /// <summary>Registra um novo usuário no sistema</summary>
    [HttpPost("registrar")]
    public async Task<IActionResult> Registrar([FromBody] RegisterDto dto)
    {
        try
        {
            var resultado = await _service.RegistrarAsync(dto);
            return Ok(resultado);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { mensagem = ex.Message });
        }
    }

    /// <summary>Autentica um usuário e retorna o token JWT</summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        try
        {
            var resultado = await _service.LoginAsync(dto);
            return Ok(resultado);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { mensagem = ex.Message });
        }
    }
}