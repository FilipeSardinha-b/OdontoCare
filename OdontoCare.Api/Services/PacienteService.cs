using OdontoCare.Api.DTOs;
using OdontoCare.Api.Models;
using OdontoCare.Api.Repositories;

namespace OdontoCare.Api.Services;

public class PacienteService : IPacienteService
{
    private readonly IPacienteRepository _repository;

    public PacienteService(IPacienteRepository repository)
    {
        _repository = repository;
    }

    public async Task<object> ObterTodosAsync(int pagina, int tamanhoPagina, string? filtroNome)
    {
        var pacientes = await _repository.ObterTodosAsync(pagina, tamanhoPagina, filtroNome);
        var total = await _repository.ContarAsync(filtroNome);

        var lista = pacientes.Select(MapearParaDto);

        return new
        {
            Total = total,
            Pagina = pagina,
            TamanhoPagina = tamanhoPagina,
            TotalPaginas = (int)Math.Ceiling((double)total / tamanhoPagina),
            Dados = lista
        };
    }

    public async Task<PacienteResponseDto?> ObterPorIdAsync(string id)
    {
        var paciente = await _repository.ObterPorIdAsync(id);
        return paciente is null ? null : MapearParaDto(paciente);
    }

    public async Task<PacienteResponseDto> CriarAsync(PacienteCreateDto dto)
    {
        var existente = await _repository.ObterPorCpfAsync(dto.CPF);
        if (existente is not null)
            throw new InvalidOperationException("Já existe um paciente com este CPF.");

        var paciente = new Paciente
        {
            NomeCompleto = dto.NomeCompleto,
            CPF = dto.CPF,
            DataNascimento = dto.DataNascimento,
            Telefone = dto.Telefone,
            Email = dto.Email,
            Endereco = dto.Endereco,
            HistoricoMedico = dto.HistoricoMedico,
            DataCadastro = DateTime.UtcNow
        };

        var criado = await _repository.CriarAsync(paciente);
        return MapearParaDto(criado);
    }

    public async Task<bool> AtualizarAsync(string id, PacienteCreateDto dto)
    {
        var existente = await _repository.ObterPorIdAsync(id);
        if (existente is null) return false;

        existente.NomeCompleto = dto.NomeCompleto;
        existente.CPF = dto.CPF;
        existente.DataNascimento = dto.DataNascimento;
        existente.Telefone = dto.Telefone;
        existente.Email = dto.Email;
        existente.Endereco = dto.Endereco;
        existente.HistoricoMedico = dto.HistoricoMedico;

        return await _repository.AtualizarAsync(id, existente);
    }

    public async Task<bool> DeletarAsync(string id)
    {
        return await _repository.DeletarAsync(id);
    }

    private static PacienteResponseDto MapearParaDto(Paciente p) => new()
    {
        Id = p.Id!,
        NomeCompleto = p.NomeCompleto,
        CPF = p.CPF,
        DataNascimento = p.DataNascimento,
        Telefone = p.Telefone,
        Email = p.Email,
        Endereco = p.Endereco,
        HistoricoMedico = p.HistoricoMedico,
        DataCadastro = p.DataCadastro
    };
}