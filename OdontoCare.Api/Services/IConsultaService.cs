using OdontoCare.Api.DTOs;

namespace OdontoCare.Api.Services;

public interface IConsultaService
{
    Task<object> ObterTodosAsync(int pagina, int tamanhoPagina, string? filtroStatus);
    Task<ConsultaResponseDto?> ObterPorIdAsync(string id);
    Task<ConsultaResponseDto> CriarAsync(ConsultaCreateDto dto);
    Task<bool> AtualizarAsync(string id, ConsultaCreateDto dto);
    Task<bool> DeletarAsync(string id);
}