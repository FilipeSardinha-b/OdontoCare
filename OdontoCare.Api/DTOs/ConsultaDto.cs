namespace OdontoCare.Api.DTOs;

public class ConsultaCreateDto
{
    public string PacienteId { get; set; } = string.Empty;
    public string Dentista { get; set; } = string.Empty;
    public string Especialidade { get; set; } = string.Empty;
    public DateTime DataConsulta { get; set; }
    public string Horario { get; set; } = string.Empty;
    public string Status { get; set; } = "Agendada";
    public string Observacoes { get; set; } = string.Empty;
    public decimal Valor { get; set; }
}

public class ConsultaResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string PacienteId { get; set; } = string.Empty;
    public string NomePaciente { get; set; } = string.Empty;
    public string Dentista { get; set; } = string.Empty;
    public string Especialidade { get; set; } = string.Empty;
    public DateTime DataConsulta { get; set; }
    public string Horario { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Observacoes { get; set; } = string.Empty;
    public decimal Valor { get; set; }
}