namespace Clinica.Modules.Consultas.Contracts;

/// <summary>
/// Contrato público do módulo Consultas. É a ÚNICA forma de outro módulo
/// consultar informações de consultas (sem acessar coleção, entidade ou repositório).
/// </summary>
public interface IConsultasApi
{
    Task<bool> PossuiConsultasAgendadasAsync(string pacienteId, CancellationToken cancellationToken = default);
}
