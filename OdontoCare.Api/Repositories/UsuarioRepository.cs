using MongoDB.Driver;
using OdontoCare.Api.Configurations;
using OdontoCare.Api.Models;
using Microsoft.Extensions.Options;

namespace OdontoCare.Api.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly IMongoCollection<Usuario> _collection;

    public UsuarioRepository(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _collection = database.GetCollection<Usuario>("usuarios");
    }

    public async Task<Usuario?> ObterPorEmailAsync(string email)
    {
        return await _collection.Find(u => u.Email == email).FirstOrDefaultAsync();
    }

    public async Task<Usuario> CriarAsync(Usuario usuario)
    {
        await _collection.InsertOneAsync(usuario);
        return usuario;
    }
}