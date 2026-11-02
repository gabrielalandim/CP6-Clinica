using Clinica.Modules.Consultas.Contracts;
using Clinica.Modules.Consultas.Domain;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Clinica.Modules.Consultas.Infrastructure;

internal sealed class ConsultasApi : IConsultasApi
{
    private readonly ConsultasMongoContext _context;

    public ConsultasApi(ConsultasMongoContext context) => _context = context;

    public async Task<bool> PossuiConsultasAgendadasAsync(string pacienteId, CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(pacienteId, out _))
            return false;

        var count = await _context.Consultas.CountDocumentsAsync(
            c => c.PacienteId == pacienteId && c.Status == StatusConsulta.Agendada,
            new CountOptions { Limit = 1 },
            cancellationToken);

        return count > 0;
    }
}
