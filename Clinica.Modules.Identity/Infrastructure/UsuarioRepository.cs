using Clinica.Modules.Identity.Application;
using Clinica.Modules.Identity.Domain;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Clinica.Modules.Identity.Infrastructure;

internal sealed class UsuarioRepository : IUsuarioRepository
{
    private readonly IdentityMongoContext _context;

    public UsuarioRepository(IdentityMongoContext context) => _context = context;

    public async Task<Usuario?> ObterPorEmailAsync(string email) =>
        await _context.Usuarios.Find(u => u.Email == email).FirstOrDefaultAsync();

    public async Task<Usuario> AdicionarAsync(Usuario usuario)
    {
        usuario.Id = ObjectId.GenerateNewId().ToString();

        try
        {
            await _context.Usuarios.InsertOneAsync(usuario);
        }
        catch (MongoWriteException ex) when (ex.WriteError?.Category == ServerErrorCategory.DuplicateKey)
        {
            throw new UsuarioJaExisteException(usuario.Email);
        }

        return usuario;
    }

    public async Task<(IReadOnlyList<Usuario> Items, int Total)> ObterTodosAsync(int page, int pageSize)
    {
        var total = await _context.Usuarios.CountDocumentsAsync(FilterDefinition<Usuario>.Empty);

        var items = await _context.Usuarios
            .Find(FilterDefinition<Usuario>.Empty)
            .SortBy(u => u.Nome)
            .ThenBy(u => u.Id)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();

        return (items, (int)total);
    }
}
