using Clinica.Modules.Consultas.Domain;
using Clinica.Modules.Pacientes.Contracts;
using Clinica.SharedKernel.Errors;
using Clinica.SharedKernel.Results;
using System.Net;

namespace Clinica.Modules.Consultas.Application;

internal sealed class ConsultaUseCase : IConsultaUseCase
{
    private readonly IConsultaRepository _repository;
    private readonly IPacientesApi _pacientes;

    public ConsultaUseCase(IConsultaRepository repository, IPacientesApi pacientes)
    {
        _repository = repository;
        _pacientes = pacientes;
    }

    public async Task<OperationResult<PageResult<GetConsultaDto>>> ObterTodosAsync(int page, int pageSize, string? pacienteId, string? status)
    {
        try
        {
            (page, pageSize) = Hateoas.Normalizar(page, pageSize);

            var (items, total) = await _repository.ObterTodosAsync(page, pageSize, pacienteId, status);

            var result = PageResult<GetConsultaDto>.Create(
                items.Select(c => c.ToDto()).ToList(), page, pageSize, total);

            var extra = string.Empty;
            if (!string.IsNullOrWhiteSpace(pacienteId)) extra += $"&pacienteId={Uri.EscapeDataString(pacienteId)}";
            if (!string.IsNullOrWhiteSpace(status)) extra += $"&status={Uri.EscapeDataString(status)}";

            result.Links = Hateoas.ForPage("/api/consultas", page, pageSize, result.TotalPages, "agendar_consulta", extra);

            return OperationResult<PageResult<GetConsultaDto>>.Success(result, (int)HttpStatusCode.OK);
        }
        catch (DomainException ex)
        {
            return OperationResult<PageResult<GetConsultaDto>>.FromException(ex);
        }
    }

    public async Task<OperationResult<GetConsultaDto>> ObterUmAsync(string id)
    {
        try
        {
            var consulta = await _repository.ObterUmAsync(id);
            return OperationResult<GetConsultaDto>.Success(consulta.ToDto(), (int)HttpStatusCode.OK);
        }
        catch (DomainException ex)
        {
            return OperationResult<GetConsultaDto>.FromException(ex);
        }
    }

    public async Task<OperationResult<GetConsultaDto>> AgendarAsync(PostConsultaDto dto)
    {
        try
        {
            // normaliza para horário local (se o cliente mandar em UTC, ex: "...Z")
            var dataHora = dto.DataHora.Kind == DateTimeKind.Utc ? dto.DataHora.ToLocalTime() : dto.DataHora;

            if (dataHora <= DateTime.Now)
                throw new DataConsultaNoPassadoException();

            if (!await _pacientes.ExisteAsync(dto.PacienteId))
                throw new PacienteDaConsultaInexistenteException(dto.PacienteId);

            var consulta = new Consulta
            {
                PacienteId = dto.PacienteId,
                DataHora = dataHora,
                Especialidade = dto.Especialidade.Trim(),
                Valor = dto.Valor,
                Status = StatusConsulta.Agendada
            };

            var salva = await _repository.AgendarAsync(consulta);

            return OperationResult<GetConsultaDto>.Success(salva.ToDto(), (int)HttpStatusCode.Created);
        }
        catch (DomainException ex)
        {
            return OperationResult<GetConsultaDto>.FromException(ex);
        }
    }

    public async Task<OperationResult<GetConsultaDto>> CancelarAsync(string id)
    {
        try
        {
            var cancelada = await _repository.CancelarAsync(id);
            return OperationResult<GetConsultaDto>.Success(cancelada.ToDto(), (int)HttpStatusCode.OK);
        }
        catch (DomainException ex)
        {
            return OperationResult<GetConsultaDto>.FromException(ex);
        }
    }
}
