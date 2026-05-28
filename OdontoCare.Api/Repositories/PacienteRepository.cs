using MongoDB.Driver;
using OdontoCare.Api.Configurations;
using OdontoCare.Api.Models;
using Microsoft.Extensions.Options;

namespace OdontoCare.Api.Repositories;

public class PacienteRepository : IPacienteRepository
{
    private readonly IMongoCollection<Paciente> _collection;

    public PacienteRepository(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _collection = database.GetCollection<Paciente>("pacientes");
    }

    public async Task<IEnumerable<Paciente>> ObterTodosAsync(int pagina, int tamanhoPagina, string? filtroNome)
    {
        var filtro = string.IsNullOrWhiteSpace(filtroNome)
            ? Builders<Paciente>.Filter.Empty
            : Builders<Paciente>.Filter.Regex(p => p.NomeCompleto,
                new MongoDB.Bson.BsonRegularExpression(filtroNome, "i"));

        return await _collection
            .Find(filtro)
            .Skip((pagina - 1) * tamanhoPagina)
            .Limit(tamanhoPagina)
            .ToListAsync();
    }

    public async Task<long> ContarAsync(string? filtroNome)
    {
        var filtro = string.IsNullOrWhiteSpace(filtroNome)
            ? Builders<Paciente>.Filter.Empty
            : Builders<Paciente>.Filter.Regex(p => p.NomeCompleto,
                new MongoDB.Bson.BsonRegularExpression(filtroNome, "i"));

        return await _collection.CountDocumentsAsync(filtro);
    }

    public async Task<Paciente?> ObterPorIdAsync(string id)
    {
        return await _collection.Find(p => p.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Paciente?> ObterPorCpfAsync(string cpf)
    {
        return await _collection.Find(p => p.CPF == cpf).FirstOrDefaultAsync();
    }

    public async Task<Paciente> CriarAsync(Paciente paciente)
    {
        await _collection.InsertOneAsync(paciente);
        return paciente;
    }

    public async Task<bool> AtualizarAsync(string id, Paciente paciente)
    {
        var resultado = await _collection.ReplaceOneAsync(p => p.Id == id, paciente);
        return resultado.ModifiedCount > 0;
    }

    public async Task<bool> DeletarAsync(string id)
    {
        var resultado = await _collection.DeleteOneAsync(p => p.Id == id);
        return resultado.DeletedCount > 0;
    }
}