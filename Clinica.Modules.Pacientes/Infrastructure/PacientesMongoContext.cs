using Clinica.Modules.Pacientes.Domain;
using MongoDB.Driver;

namespace Clinica.Modules.Pacientes.Infrastructure;

internal sealed class PacientesMongoContext
{
    public const string CollectionName = "pacientes";

    public IMongoCollection<Paciente> Pacientes { get; }

    public PacientesMongoContext(IMongoDatabase database)
    {
        Pacientes = database.GetCollection<Paciente>(CollectionName);
    }

    public async Task EnsureIndexesAsync(CancellationToken ct = default)
    {
        var keys = Builders<Paciente>.IndexKeys;

        await Pacientes.Indexes.CreateManyAsync(new[]
        {
            new CreateIndexModel<Paciente>(keys.Ascending(p => p.Email),
                new CreateIndexOptions { Unique = true, Name = "ux_pacientes_email" }),
            new CreateIndexModel<Paciente>(keys.Ascending(p => p.Nome),
                new CreateIndexOptions { Name = "ix_pacientes_nome" })
        }, ct);
    }
}
