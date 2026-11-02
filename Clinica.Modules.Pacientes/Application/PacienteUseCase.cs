using Clinica.Modules.Consultas.Contracts;
using Clinica.Modules.Pacientes.Domain;
using Clinica.SharedKernel.Errors;
using Clinica.SharedKernel.Results;
using System.Net;

namespace Clinica.Modules.Pacientes.Application;

internal sealed class PacienteUseCase : IPacienteUseCase
{
    private readonly IPacienteRepository _repository;
    private readonly IConsultasApi _consultas;

    public PacienteUseCase(IPacienteRepository repository, IConsultasApi consultas)
    {
        _repository = repository;
        _consultas = consultas;
    }

    public async Task<OperationResult<PageResult<GetPacienteDto>>> ObterTodosAsync(int page, int pageSize, string? nome)
    {
        try
        {
            (page, pageSize) = Hateoas.Normalizar(page, pageSize);

            var (items, total) = await _repository.ObterTodosAsync(page, pageSize, nome);

            var result = PageResult<GetPacienteDto>.Create(
                items.Select(p => p.ToDto()).ToList(), page, pageSize, total);

            var extra = string.IsNullOrWhiteSpace(nome) ? string.Empty : $"&nome={Uri.EscapeDataString(nome.Trim())}";
            result.Links = Hateoas.ForPage("/api/pacientes", page, pageSize, result.TotalPages, "create_paciente", extra);

            return OperationResult<PageResult<GetPacienteDto>>.Success(result, (int)HttpStatusCode.OK);
        }
        catch (DomainException ex)
        {
            return OperationResult<PageResult<GetPacienteDto>>.FromException(ex);
        }
    }

    public async Task<OperationResult<GetPacienteDto>> ObterUmAsync(string id)
    {
        try
        {
            var paciente = await _repository.ObterUmAsync(id);
            return OperationResult<GetPacienteDto>.Success(paciente.ToDto(), (int)HttpStatusCode.OK);
        }
        catch (DomainException ex)
        {
            return OperationResult<GetPacienteDto>.FromException(ex);
        }
    }

    public async Task<OperationResult<GetPacienteDto>> AdicionarAsync(PostPacienteDto dto)
    {
        try
        {
            var salvo = await _repository.AdicionarAsync(dto.ToEntity());
            return OperationResult<GetPacienteDto>.Success(salvo.ToDto(), (int)HttpStatusCode.Created);
        }
        catch (DomainException ex)
        {
            return OperationResult<GetPacienteDto>.FromException(ex);
        }
    }

    public async Task<OperationResult<GetPacienteDto>> AtualizarAsync(string id, PutPacienteDto dto)
    {
        try
        {
            var atualizado = await _repository.AtualizarAsync(id, dto.ToEntity());
            return OperationResult<GetPacienteDto>.Success(atualizado.ToDto(), (int)HttpStatusCode.OK);
        }
        catch (DomainException ex)
        {
            return OperationResult<GetPacienteDto>.FromException(ex);
        }
    }

    public async Task<OperationResult<bool>> DeletarAsync(string id)
    {
        try
        {
            // valida o id e garante que o paciente existe (lança 400/404)
            await _repository.ObterUmAsync(id);

            // regra entre módulos: conversa com Consultas apenas pelo contrato
            if (await _consultas.PossuiConsultasAgendadasAsync(id))
                throw new PacienteComConsultasAgendadasException(id);

            await _repository.DeletarAsync(id);
            return OperationResult<bool>.Success(true, (int)HttpStatusCode.NoContent);
        }
        catch (DomainException ex)
        {
            return OperationResult<bool>.FromException(ex);
        }
    }
}
