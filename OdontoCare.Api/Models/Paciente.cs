using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OdontoCare.Api.Models;

public class Paciente
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("nomeCompleto")]
    public string NomeCompleto { get; set; } = string.Empty;

    [BsonElement("cpf")]
    public string CPF { get; set; } = string.Empty;

    [BsonElement("dataNascimento")]
    public DateTime DataNascimento { get; set; }

    [BsonElement("telefone")]
    public string Telefone { get; set; } = string.Empty;

    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;

    [BsonElement("endereco")]
    public string Endereco { get; set; } = string.Empty;

    [BsonElement("historicoMedico")]
    public string HistoricoMedico { get; set; } = string.Empty;

    [BsonElement("dataCadastro")]
    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
}