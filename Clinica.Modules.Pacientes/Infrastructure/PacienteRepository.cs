using Clinica.Modules.Pacientes.Application;
using Clinica.Modules.Pacientes.Domain;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.RegularExpressions;

namespace Clinica.Modules.Pacientes.Infrastructure;

internal sealed class PacienteRepository : IPacienteRepository
{
    private readonly PacientesMongoContext _context;

    public PacienteRepository(PacientesMongoContext context) => _context = context;

    public async Task<(IReadOnlyList<Paciente> Items, int Total)> ObterTodosAsync(int page, int pageSize, string? nome)
    {
        var filter = Builders<Paciente>.Filter.Empty;

        if (!string.IsNullOrWhiteSpace(nome))
        {
            var regex = new BsonRegularExpression(Regex.Escape(nome.Trim()), "i");
            filter = Builders<Paciente>.Filter.Regex(p => p.Nome, regex);
        }

        var total = await _context.Pacientes.CountDocumentsAsync(filter);

        var items = await _context.Pacientes
            .Find(filter)
            .SortBy(p => p.Nome)
            .ThenBy(p => p.Id)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();

        return (items, (int)total);
    }

    public async Task<Paciente> ObterUmAsync(string id)
    {
        GarantirIdValido(id);

        var paciente = await _context.Pacientes.Find(p => p.Id == id).FirstOrDefaultAsync();

        return paciente ?? throw new PacienteNaoEncontradoException(id);
    }

    public async Task<Paciente> AdicionarAsync(Paciente paciente)
    {
        paciente.Id = ObjectId.GenerateNewId().ToString();
        paciente.CriadoEm = DateTime.UtcNow;

        try
        {
            await _context.Pacientes.InsertOneAsync(paciente);
        }
        catch (MongoWriteException ex) when (ex.WriteError?.Category == ServerErrorCategory.DuplicateKey)
        {
            throw new PacienteEmailJaCadastradoException(paciente.Email);
        }

        return paciente;
    }

    public async Task<Paciente> AtualizarAsync(string id, Paciente dados)
    {
        GarantirIdValido(id);

        var emailEmUso = await _context.Pacientes.CountDocumentsAsync(p => p.Email == dados.Email && p.Id != id);
        if (emailEmUso > 0)
            throw new PacienteEmailJaCadastradoException(dados.Email);

        var update = Builders<Paciente>.Update
            .Set(p => p.Nome, dados.Nome)
            .Set(p => p.Email, dados.Email)
            .Set(p => p.Telefone, dados.Telefone)
            .Set(p => p.Cidade, dados.Cidade)
            .Set(p => p.Uf, dados.Uf);

        try
        {
            var result = await _context.Pacientes.UpdateOneAsync(p => p.Id == id, update);

            if (result.MatchedCount == 0)
                throw new PacienteNaoEncontradoException(id);
        }
        catch (MongoWriteException ex) when (ex.WriteError?.Category == ServerErrorCategory.DuplicateKey)
        {
            throw new PacienteEmailJaCadastradoException(dados.Email);
        }

        return await ObterUmAsync(id);
    }

    public async Task DeletarAsync(string id)
    {
        GarantirIdValido(id);

        var result = await _context.Pacientes.DeleteOneAsync(p => p.Id == id);

        if (result.DeletedCount == 0)
            throw new PacienteNaoEncontradoException(id);
    }

    private static void GarantirIdValido(string id)
    {
        if (!ObjectId.TryParse(id, out _))
            throw new PacienteIdInvalidoException(id);
    }
}
