using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OdontoCare.Api.Models;

public class Consulta
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("pacienteId")]
    public string PacienteId { get; set; } = string.Empty;

    [BsonElement("dentista")]
    public string Dentista { get; set; } = string.Empty;

    [BsonElement("especialidade")]
    public string Especialidade { get; set; } = string.Empty;

    [BsonElement("dataConsulta")]
    public DateTime DataConsulta { get; set; }

    [BsonElement("horario")]
    public string Horario { get; set; } = string.Empty;

    [BsonElement("status")]
    public string Status { get; set; } = "Agendada";

    [BsonElement("observacoes")]
    public string Observacoes { get; set; } = string.Empty;

    [BsonElement("valor")]
    public decimal Valor { get; set; }
}