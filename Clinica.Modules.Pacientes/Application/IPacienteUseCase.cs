using Clinica.SharedKernel.Results;

namespace Clinica.Modules.Pacientes.Application;

public interface IPacienteUseCase
{
    Task<OperationResult<PageResult<GetPacienteDto>>> ObterTodosAsync(int page, int pageSize, string? nome);
    Task<OperationResult<GetPacienteDto>> ObterUmAsync(string id);
    Task<OperationResult<GetPacienteDto>> AdicionarAsync(PostPacienteDto dto);
    Task<OperationResult<GetPacienteDto>> AtualizarAsync(string id, PutPacienteDto dto);
    Task<OperationResult<bool>> DeletarAsync(string id);
}
