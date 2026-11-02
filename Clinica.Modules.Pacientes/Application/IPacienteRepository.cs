using Clinica.Modules.Pacientes.Domain;

namespace Clinica.Modules.Pacientes.Application;

internal interface IPacienteRepository
{
    Task<(IReadOnlyList<Paciente> Items, int Total)> ObterTodosAsync(int page, int pageSize, string? nome);
    Task<Paciente> ObterUmAsync(string id);
    Task<Paciente> AdicionarAsync(Paciente paciente);
    Task<Paciente> AtualizarAsync(string id, Paciente dados);
    Task DeletarAsync(string id);
}
