using Clinica.SharedKernel.Results;

namespace Clinica.Modules.Consultas.Application;

public interface IConsultaUseCase
{
    Task<OperationResult<PageResult<GetConsultaDto>>> ObterTodosAsync(int page, int pageSize, string? pacienteId, string? status);
    Task<OperationResult<GetConsultaDto>> ObterUmAsync(string id);
    Task<OperationResult<GetConsultaDto>> AgendarAsync(PostConsultaDto dto);
    Task<OperationResult<GetConsultaDto>> CancelarAsync(string id);
}
