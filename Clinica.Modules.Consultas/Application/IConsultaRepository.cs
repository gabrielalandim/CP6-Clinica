using Clinica.Modules.Consultas.Domain;

namespace Clinica.Modules.Consultas.Application;

internal interface IConsultaRepository
{
    Task<(IReadOnlyList<Consulta> Items, int Total)> ObterTodosAsync(int page, int pageSize, string? pacienteId, string? status);
    Task<Consulta> ObterUmAsync(string id);
    Task<Consulta> AgendarAsync(Consulta consulta);
    Task<Consulta> CancelarAsync(string id);
}
