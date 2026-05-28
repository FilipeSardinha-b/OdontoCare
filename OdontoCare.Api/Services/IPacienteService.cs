using OdontoCare.Api.DTOs;

namespace OdontoCare.Api.Services;

public interface IPacienteService
{
    Task<object> ObterTodosAsync(int pagina, int tamanhoPagina, string? filtroNome);
    Task<PacienteResponseDto?> ObterPorIdAsync(string id);
    Task<PacienteResponseDto> CriarAsync(PacienteCreateDto dto);
    Task<bool> AtualizarAsync(string id, PacienteCreateDto dto);
    Task<bool> DeletarAsync(string id);
}