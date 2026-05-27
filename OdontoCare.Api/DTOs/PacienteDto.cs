namespace OdontoCare.Api.DTOs;

public class PacienteCreateDto
{
    public string NomeCompleto { get; set; } = string.Empty;
    public string CPF { get; set; } = string.Empty;
    public DateTime DataNascimento { get; set; }
    public string Telefone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public string HistoricoMedico { get; set; } = string.Empty;
}

public class PacienteResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string NomeCompleto { get; set; } = string.Empty;
    public string CPF { get; set; } = string.Empty;
    public DateTime DataNascimento { get; set; }
    public string Telefone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public string HistoricoMedico { get; set; } = string.Empty;
    public DateTime DataCadastro { get; set; }
}