using Clinica.Modules.Identity.Domain;
using Clinica.SharedKernel.Modules;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Clinica.Modules.Identity.Infrastructure;

internal sealed class IdentityInitializer : IModuleInitializer
{
    private readonly IdentityMongoContext _context;

    public IdentityInitializer(IdentityMongoContext context) => _context = context;

    public string ModuleName => "Identity";

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await _context.EnsureIndexesAsync(cancellationToken);

        await SeedAsync("Administrador", "admin@clinica.com", "Admin@123", Roles.Admin, cancellationToken);
        await SeedAsync("Usuário Comum", "user@clinica.com", "User@123", Roles.User, cancellationToken);
    }

    private async Task SeedAsync(string nome, string email, string senha, string role, CancellationToken ct)
    {
        var existe = await _context.Usuarios.CountDocumentsAsync(
            u => u.Email == email, new CountOptions { Limit = 1 }, ct);

        if (existe > 0) return;

        await _context.Usuarios.InsertOneAsync(new Usuario
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Nome = nome,
            Email = email,
            PasswordHash = PasswordHasher.Hash(senha),
            Role = role
        }, cancellationToken: ct);
    }
}
