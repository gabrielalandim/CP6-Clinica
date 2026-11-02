using Clinica.Modules.Identity.Domain;
using MongoDB.Driver;

namespace Clinica.Modules.Identity.Infrastructure;

internal sealed class IdentityMongoContext
{
    public const string CollectionName = "usuarios";

    public IMongoCollection<Usuario> Usuarios { get; }

    public IdentityMongoContext(IMongoDatabase database)
    {
        Usuarios = database.GetCollection<Usuario>(CollectionName);
    }

    public Task EnsureIndexesAsync(CancellationToken ct = default) =>
        Usuarios.Indexes.CreateOneAsync(
            new CreateIndexModel<Usuario>(
                Builders<Usuario>.IndexKeys.Ascending(u => u.Email),
                new CreateIndexOptions { Unique = true, Name = "ux_usuarios_email" }),
            cancellationToken: ct);
}
