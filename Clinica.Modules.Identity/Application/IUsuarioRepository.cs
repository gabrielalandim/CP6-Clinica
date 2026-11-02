using Clinica.Modules.Identity.Domain;

namespace Clinica.Modules.Identity.Application;

internal interface IUsuarioRepository
{
    Task<Usuario?> ObterPorEmailAsync(string email);
    Task<Usuario> AdicionarAsync(Usuario usuario);
    Task<(IReadOnlyList<Usuario> Items, int Total)> ObterTodosAsync(int page, int pageSize);
}
