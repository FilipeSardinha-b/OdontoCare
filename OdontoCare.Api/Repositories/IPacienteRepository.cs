using OdontoCare.Api.Models;

namespace OdontoCare.Api.Repositories;

public interface IPacienteRepository
{
    Task<IEnumerable<Paciente>> ObterTodosAsync(int pagina, int tamanhoPagina, string? filtroNome);
    Task<long> ContarAsync(string? filtroNome);
    Task<Paciente?> ObterPorIdAsync(string id);
    Task<Paciente?> ObterPorCpfAsync(string cpf);
    Task<Paciente> CriarAsync(Paciente paciente);
    Task<bool> AtualizarAsync(string id, Paciente paciente);
    Task<bool> DeletarAsync(string id);
}