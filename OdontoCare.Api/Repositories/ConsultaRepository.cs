using MongoDB.Driver;
using OdontoCare.Api.Configurations;
using OdontoCare.Api.Models;
using Microsoft.Extensions.Options;

namespace OdontoCare.Api.Repositories;

public class ConsultaRepository : IConsultaRepository
{
    private readonly IMongoCollection<Consulta> _collection;

    public ConsultaRepository(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _collection = database.GetCollection<Consulta>("consultas");
    }

    public async Task<IEnumerable<Consulta>> ObterTodosAsync(int pagina, int tamanhoPagina, string? filtroStatus)
    {
        var filtro = string.IsNullOrWhiteSpace(filtroStatus)
            ? Builders<Consulta>.Filter.Empty
            : Builders<Consulta>.Filter.Eq(c => c.Status, filtroStatus);

        return await _collection
            .Find(filtro)
            .Skip((pagina - 1) * tamanhoPagina)
            .Limit(tamanhoPagina)
            .ToListAsync();
    }

    public async Task<long> ContarAsync(string? filtroStatus)
    {
        var filtro = string.IsNullOrWhiteSpace(filtroStatus)
            ? Builders<Consulta>.Filter.Empty
            : Builders<Consulta>.Filter.Eq(c => c.Status, filtroStatus);

        return await _collection.CountDocumentsAsync(filtro);
    }

    public async Task<Consulta?> ObterPorIdAsync(string id)
    {
        return await _collection.Find(c => c.Id == id).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Consulta>> ObterPorPacienteIdAsync(string pacienteId)
    {
        return await _collection.Find(c => c.PacienteId == pacienteId).ToListAsync();
    }

    public async Task<Consulta> CriarAsync(Consulta consulta)
    {
        await _collection.InsertOneAsync(consulta);
        return consulta;
    }

    public async Task<bool> AtualizarAsync(string id, Consulta consulta)
    {
        var resultado = await _collection.ReplaceOneAsync(c => c.Id == id, consulta);
        return resultado.ModifiedCount > 0;
    }

    public async Task<bool> DeletarAsync(string id)
    {
        var resultado = await _collection.DeleteOneAsync(c => c.Id == id);
        return resultado.DeletedCount > 0;
    }
}