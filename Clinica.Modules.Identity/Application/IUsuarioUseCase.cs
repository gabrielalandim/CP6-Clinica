using Clinica.SharedKernel.Results;

namespace Clinica.Modules.Identity.Application;

public interface IUsuarioUseCase
{
    Task<OperationResult<GetUserDto>> RegistrarAsync(PostUserDto dto);
    Task<OperationResult<GetAuthDto>> AutenticarAsync(PostAuthDto dto);
    Task<OperationResult<PageResult<GetUserDto>>> ListarAsync(int page, int pageSize);
}
