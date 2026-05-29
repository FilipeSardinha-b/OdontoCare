using OdontoCare.Api.Models;

namespace OdontoCare.Api.Repositories;

public interface IUsuarioRepository
{
    Task<Usuario?> ObterPorEmailAsync(string email);
    Task<Usuario> CriarAsync(Usuario usuario);
}