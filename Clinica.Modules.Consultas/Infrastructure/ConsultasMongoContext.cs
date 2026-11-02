using Clinica.Modules.Consultas.Domain;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Clinica.Modules.Consultas.Infrastructure;

internal sealed class ConsultasMongoContext
{
    public const string CollectionName = "consultas";

    public IMongoCollection<Consulta> Consultas { get; }

    public ConsultasMongoContext(IMongoDatabase database)
    {
        Consultas = database.GetCollection<Consulta>(CollectionName);
    }

    public async Task EnsureIndexesAsync(CancellationToken ct = default)
    {
        var keys = Builders<Consulta>.IndexKeys;

        await Consultas.Indexes.CreateManyAsync(new[]
        {
            new CreateIndexModel<Consulta>(
                keys.Ascending(c => c.DataHora).Ascending(c => c.Especialidade),
                new CreateIndexOptions<Consulta>
                {
                    Unique = true,
                    Name = "ux_consultas_horario_agendado",
                    PartialFilterExpression = new BsonDocumentFilterDefinition<Consulta>(
                        new BsonDocument("status", StatusConsulta.Agendada))
                }),
            new CreateIndexModel<Consulta>(
                keys.Ascending(c => c.PacienteId),
                new CreateIndexOptions { Name = "ix_consultas_paciente" })
        }, ct);
    }
}
