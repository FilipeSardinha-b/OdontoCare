using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OdontoCare.Api.Configurations;
using OdontoCare.Api.DTOs;
using OdontoCare.Api.Models;
using OdontoCare.Api.Repositories;

namespace OdontoCare.Api.Services;

// SRP: responsável apenas pela lógica de autenticação e geração de token
public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _repository;
    private readonly JwtSettings _jwtSettings;

    public AuthService(IUsuarioRepository repository, IOptions<JwtSettings> jwtSettings)
    {
        _repository = repository;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<AuthResponseDto> RegistrarAsync(RegisterDto dto)
    {
        // Verifica se o email já está em uso
        var existente = await _repository.ObterPorEmailAsync(dto.Email);
        if (existente is not null)
            throw new InvalidOperationException("Email já cadastrado.");

        // Criptografa a senha com BCrypt
        var usuario = new Usuario
        {
            Email = dto.Email,
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha),
            Role = dto.Role == "admin" ? "admin" : "usuario",
            DataCadastro = DateTime.UtcNow
        };

        await _repository.CriarAsync(usuario);
        return GerarToken(usuario);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var usuario = await _repository.ObterPorEmailAsync(dto.Email);

        // Verifica se o usuário existe e se a senha está correta
        if (usuario is null || !BCrypt.Net.BCrypt.Verify(dto.Senha, usuario.SenhaHash))
            throw new UnauthorizedAccessException("Email ou senha inválidos.");

        return GerarToken(usuario);
    }

    // Gera o JWT com as claims do usuário (email + role)
    private AuthResponseDto GerarToken(Usuario usuario)
    {
        var expiracao = DateTime.UtcNow.AddHours(_jwtSettings.ExpirationHours);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id!),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Role, usuario.Role)
        };

        var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expiracao,
            signingCredentials: credenciais
        );

        return new AuthResponseDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Email = usuario.Email,
            Role = usuario.Role,
            Expiracao = expiracao
        };
    }
}