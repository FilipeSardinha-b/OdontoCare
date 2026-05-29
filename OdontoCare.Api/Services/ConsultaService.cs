using OdontoCare.Api.DTOs;
using OdontoCare.Api.Models;
using OdontoCare.Api.Repositories;

namespace OdontoCare.Api.Services;

// SRP: esta classe só cuida das regras de negócio de Consulta
public class ConsultaService : IConsultaService
{
    private readonly IConsultaRepository _consultaRepository;
    private readonly IPacienteRepository _pacienteRepository;

    // DIP: depende de abstrações, não de implementações concretas
    public ConsultaService(
        IConsultaRepository consultaRepository,
        IPacienteRepository pacienteRepository)
    {
        _consultaRepository = consultaRepository;
        _pacienteRepository = pacienteRepository;
    }

    public async Task<object> ObterTodosAsync(int pagina, int tamanhoPagina, string? filtroStatus)
    {
        var consultas = await _consultaRepository.ObterTodosAsync(pagina, tamanhoPagina, filtroStatus);
        var total = await _consultaRepository.ContarAsync(filtroStatus);

        // Enriquece cada consulta com o nome do paciente
        var lista = new List<ConsultaResponseDto>();
        foreach (var c in consultas)
        {
            var paciente = await _pacienteRepository.ObterPorIdAsync(c.PacienteId);
            lista.Add(MapearParaDto(c, paciente?.NomeCompleto ?? "Paciente não encontrado"));
        }

        return new
        {
            Total = total,
            Pagina = pagina,
            TamanhoPagina = tamanhoPagina,
            TotalPaginas = (int)Math.Ceiling((double)total / tamanhoPagina),
            Dados = lista
        };
    }

    public async Task<ConsultaResponseDto?> ObterPorIdAsync(string id)
    {
        var consulta = await _consultaRepository.ObterPorIdAsync(id);
        if (consulta is null) return null;

        var paciente = await _pacienteRepository.ObterPorIdAsync(consulta.PacienteId);
        return MapearParaDto(consulta, paciente?.NomeCompleto ?? "Paciente não encontrado");
    }

    public async Task<ConsultaResponseDto> CriarAsync(ConsultaCreateDto dto)
    {
        // Regra de negócio: o paciente deve existir
        var paciente = await _pacienteRepository.ObterPorIdAsync(dto.PacienteId);
        if (paciente is null)
            throw new InvalidOperationException("Paciente não encontrado.");

        // Regra de negócio: status válidos
        var statusValidos = new[] { "Agendada", "Confirmada", "Realizada", "Cancelada" };
        if (!statusValidos.Contains(dto.Status))
            throw new InvalidOperationException($"Status inválido. Use: {string.Join(", ", statusValidos)}");

        var consulta = new Consulta
        {
            PacienteId = dto.PacienteId,
            Dentista = dto.Dentista,
            Especialidade = dto.Especialidade,
            DataConsulta = dto.DataConsulta,
            Horario = dto.Horario,
            Status = dto.Status,
            Observacoes = dto.Observacoes,
            Valor = dto.Valor
        };

        var criada = await _consultaRepository.CriarAsync(consulta);
        return MapearParaDto(criada, paciente.NomeCompleto);
    }

    public async Task<bool> AtualizarAsync(string id, ConsultaCreateDto dto)
    {
        var existente = await _consultaRepository.ObterPorIdAsync(id);
        if (existente is null) return false;

        var paciente = await _pacienteRepository.ObterPorIdAsync(dto.PacienteId);
        if (paciente is null)
            throw new InvalidOperationException("Paciente não encontrado.");

        existente.PacienteId = dto.PacienteId;
        existente.Dentista = dto.Dentista;
        existente.Especialidade = dto.Especialidade;
        existente.DataConsulta = dto.DataConsulta;
        existente.Horario = dto.Horario;
        existente.Status = dto.Status;
        existente.Observacoes = dto.Observacoes;
        existente.Valor = dto.Valor;

        return await _consultaRepository.AtualizarAsync(id, existente);
    }

    public async Task<bool> DeletarAsync(string id)
    {
        return await _consultaRepository.DeletarAsync(id);
    }

    private static ConsultaResponseDto MapearParaDto(Consulta c, string nomePaciente) => new()
    {
        Id = c.Id!,
        PacienteId = c.PacienteId,
        NomePaciente = nomePaciente,
        Dentista = c.Dentista,
        Especialidade = c.Especialidade,
        DataConsulta = c.DataConsulta,
        Horario = c.Horario,
        Status = c.Status,
        Observacoes = c.Observacoes,
        Valor = c.Valor
    };
}