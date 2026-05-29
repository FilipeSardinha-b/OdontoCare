using Microsoft.AspNetCore.Mvc;
using OdontoCare.Api.DTOs;
using OdontoCare.Api.Services;

namespace OdontoCare.Api.Controllers;

/// <summary>Endpoints de autenticação</summary>
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _service;

    public AuthController(IAuthService service)
    {
        _service = service;
    }

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