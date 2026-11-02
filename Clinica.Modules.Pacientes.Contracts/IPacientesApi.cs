namespace Clinica.Modules.Pacientes.Contracts;

/// <summary>
/// Contrato público do módulo Pacientes. É a ÚNICA forma de outro módulo
/// consultar informações de pacientes (sem acessar coleção, entidade ou repositório).
/// </summary>
public interface IPacientesApi
{
    Task<bool> ExisteAsync(string pacienteId, CancellationToken cancellationToken = default);
}
