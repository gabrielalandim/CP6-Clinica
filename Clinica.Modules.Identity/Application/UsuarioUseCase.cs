using Clinica.Modules.Identity.Domain;
using Clinica.SharedKernel.Errors;
using Clinica.SharedKernel.Results;
using System.Net;

namespace Clinica.Modules.Identity.Application;

internal sealed class UsuarioUseCase : IUsuarioUseCase
{
    private readonly IUsuarioRepository _repository;
    private readonly JwtTokenService _tokenService;

    public UsuarioUseCase(IUsuarioRepository repository, JwtTokenService tokenService)
    {
        _repository = repository;
        _tokenService = tokenService;
    }

    public async Task<OperationResult<GetUserDto>> RegistrarAsync(PostUserDto dto)
    {
        try
        {
            var usuario = new Usuario
            {
                Nome = dto.Nome.Trim(),
                Email = dto.Email.Trim().ToLowerInvariant(),
                PasswordHash = PasswordHasher.Hash(dto.Password),
                Role = Roles.User
            };

            var salvo = await _repository.AdicionarAsync(usuario);

            return OperationResult<GetUserDto>.Success(ToDto(salvo), (int)HttpStatusCode.Created);
        }
        catch (DomainException ex)
        {
            return OperationResult<GetUserDto>.FromException(ex);
        }
    }

    public async Task<OperationResult<GetAuthDto>> AutenticarAsync(PostAuthDto dto)
    {
        try
        {
            var usuario = await _repository.ObterPorEmailAsync(dto.Email.Trim().ToLowerInvariant());

            if (usuario is null || !PasswordHasher.Verify(dto.Senha, usuario.PasswordHash))
                throw new CredenciaisInvalidasException();

            var auth = new GetAuthDto
            {
                Email = usuario.Email,
                Role = usuario.Role,
                Jwt = _tokenService.Gerar(usuario)
            };

            return OperationResult<GetAuthDto>.Success(auth, (int)HttpStatusCode.OK);
        }
        catch (DomainException ex)
        {
            return OperationResult<GetAuthDto>.FromException(ex);
        }
    }

    public async Task<OperationResult<PageResult<GetUserDto>>> ListarAsync(int page, int pageSize)
    {
        try
        {
            (page, pageSize) = Hateoas.Normalizar(page, pageSize);

            var (items, total) = await _repository.ObterTodosAsync(page, pageSize);

            var result = PageResult<GetUserDto>.Create(items.Select(ToDto).ToList(), page, pageSize, total);
            result.Links = Hateoas.ForPage("/api/user", page, pageSize, result.TotalPages, "register_user");

            return OperationResult<PageResult<GetUserDto>>.Success(result, (int)HttpStatusCode.OK);
        }
        catch (DomainException ex)
        {
            return OperationResult<PageResult<GetUserDto>>.FromException(ex);
        }
    }

    private static GetUserDto ToDto(Usuario u) => new()
    {
        Id = u.Id,
        Nome = u.Nome,
        Email = u.Email,
        Role = u.Role
    };
}
