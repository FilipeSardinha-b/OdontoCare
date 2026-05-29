using OdontoCare.Api.Models;

namespace OdontoCare.Api.Repositories;

public interface IConsultaRepository
{
    Task<IEnumerable<Consulta>> ObterTodosAsync(int pagina, int tamanhoPagina, string? filtroStatus);
    Task<long> ContarAsync(string? filtroStatus);
    Task<Consulta?> ObterPorIdAsync(string id);
    Task<IEnumerable<Consulta>> ObterPorPacienteIdAsync(string pacienteId);
    Task<Consulta> CriarAsync(Consulta consulta);
    Task<bool> AtualizarAsync(string id, Consulta consulta);
    Task<bool> DeletarAsync(string id);
}