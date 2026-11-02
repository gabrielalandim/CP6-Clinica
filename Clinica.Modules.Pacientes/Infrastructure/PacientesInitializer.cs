using Clinica.Modules.Pacientes.Domain;
using Clinica.SharedKernel.Modules;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Clinica.Modules.Pacientes.Infrastructure;

internal sealed class PacientesInitializer : IModuleInitializer
{
    private readonly PacientesMongoContext _context;

    public PacientesInitializer(PacientesMongoContext context) => _context = context;

    public string ModuleName => "Pacientes";

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await _context.EnsureIndexesAsync(cancellationToken);

        if (await _context.Pacientes.CountDocumentsAsync(FilterDefinition<Paciente>.Empty, cancellationToken: cancellationToken) > 0)
            return;

        var nomes = new[]
        {
            "Ana Souza", "Bruno Lima", "Carla Dias", "Daniel Rocha", "Elisa Prado", "Fabio Nunes",
            "Gabriela Alves", "Henrique Melo", "Isabela Costa", "João Pereira", "Karina Teixeira", "Lucas Barros"
        };

        var seed = nomes.Select((nome, i) => new Paciente
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Nome = nome,
            Email = $"{nome.Split(' ')[0].ToLowerInvariant()}@exemplo.com",
            Telefone = $"(11) 9{i:0000}-000{i % 10}",
            Cidade = i % 2 == 0 ? "São Paulo" : "Campinas",
            Uf = "SP",
            CriadoEm = DateTime.UtcNow
        });

        await _context.Pacientes.InsertManyAsync(seed, cancellationToken: cancellationToken);
    }
}
