using Clinica.Modules.Pacientes.Contracts;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Clinica.Modules.Pacientes.Infrastructure;

internal sealed class PacientesApi : IPacientesApi
{
    private readonly PacientesMongoContext _context;

    public PacientesApi(PacientesMongoContext context) => _context = context;

    public async Task<bool> ExisteAsync(string pacienteId, CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(pacienteId, out _))
            return false;

        var count = await _context.Pacientes.CountDocumentsAsync(
            p => p.Id == pacienteId,
            new CountOptions { Limit = 1 },
            cancellationToken);

        return count > 0;
    }
}
