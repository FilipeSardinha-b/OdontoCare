using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OdontoCare.Api.Models;

public class Usuario
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;

    [BsonElement("senhaHash")]
    public string SenhaHash { get; set; } = string.Empty;

    [BsonElement("role")]
    public string Role { get; set; } = "usuario";

    [BsonElement("dataCadastro")]
    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
}