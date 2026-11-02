using Clinica.Modules.Consultas.Application;
using Clinica.Modules.Consultas.Domain;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Clinica.Modules.Consultas.Infrastructure;

internal sealed class ConsultaRepository : IConsultaRepository
{
    private readonly ConsultasMongoContext _context;

    public ConsultaRepository(ConsultasMongoContext context) => _context = context;

    public async Task<(IReadOnlyList<Consulta> Items, int Total)> ObterTodosAsync(int page, int pageSize, string? pacienteId, string? status)
    {
        var builder = Builders<Consulta>.Filter;
        var filter = builder.Empty;

        if (!string.IsNullOrWhiteSpace(pacienteId))
        {
            if (!ObjectId.TryParse(pacienteId, out _))
                return (Array.Empty<Consulta>(), 0);

            filter &= builder.Eq(c => c.PacienteId, pacienteId);
        }

        if (!string.IsNullOrWhiteSpace(status))
            filter &= builder.Eq(c => c.Status, status.Trim());

        var total = await _context.Consultas.CountDocumentsAsync(filter);

        var items = await _context.Consultas
            .Find(filter)
            .SortBy(c => c.DataHora)
            .ThenBy(c => c.Id)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();

        return (items, (int)total);
    }

    public async Task<Consulta> ObterUmAsync(string id)
    {
        GarantirIdValido(id);

        var consulta = await _context.Consultas.Find(c => c.Id == id).FirstOrDefaultAsync();

        return consulta ?? throw new ConsultaNaoEncontradaException(id);
    }

    public async Task<Consulta> AgendarAsync(Consulta consulta)
    {
        var ocupado = await _context.Consultas.CountDocumentsAsync(
            c => c.DataHora == consulta.DataHora
              && c.Especialidade == consulta.Especialidade
              && c.Status == StatusConsulta.Agendada,
            new CountOptions { Limit = 1 });

        if (ocupado > 0)
            throw new HorarioIndisponivelException(consulta.Especialidade, consulta.DataHora);

        consulta.Id = ObjectId.GenerateNewId().ToString();

        try
        {
            await _context.Consultas.InsertOneAsync(consulta);
        }
        catch (MongoWriteException ex) when (ex.WriteError?.Category == ServerErrorCategory.DuplicateKey)
        {
            throw new HorarioIndisponivelException(consulta.Especialidade, consulta.DataHora);
        }

        return consulta;
    }

    public async Task<Consulta> CancelarAsync(string id)
    {
        GarantirIdValido(id);

        var cancelada = await _context.Consultas.FindOneAndUpdateAsync<Consulta>(
            c => c.Id == id && c.Status == StatusConsulta.Agendada,
            Builders<Consulta>.Update.Set(c => c.Status, StatusConsulta.Cancelada),
            new FindOneAndUpdateOptions<Consulta> { ReturnDocument = ReturnDocument.After });

        if (cancelada is not null)
            return cancelada;

        var existe = await _context.Consultas.CountDocumentsAsync(c => c.Id == id, new CountOptions { Limit = 1 });

        if (existe > 0)
            throw new ConsultaJaCanceladaException(id);

        throw new ConsultaNaoEncontradaException(id);
    }

    private static void GarantirIdValido(string id)
    {
        if (!ObjectId.TryParse(id, out _))
            throw new ConsultaIdInvalidoException(id);
    }
}
