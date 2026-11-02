using Clinica.SharedKernel.Errors;

namespace Clinica.Modules.Consultas.Domain;

public sealed class ConsultaNaoEncontradaException : DomainException
{
    public ConsultaNaoEncontradaException(string id)
        : base("CONSULTA_NAO_ENCONTRADA", $"Consulta '{id}' não encontrada.", 404) { }
}

public sealed class ConsultaIdInvalidoException : DomainException
{
    public ConsultaIdInvalidoException(string id)
        : base("CONSULTA_ID_INVALIDO", $"O id '{id}' não é um identificador válido.", 400) { }
}

public sealed class HorarioIndisponivelException : DomainException
{
    public HorarioIndisponivelException(string especialidade, DateTime dataHora)
        : base("HORARIO_INDISPONIVEL",
               $"Já existe consulta de {especialidade} agendada para {dataHora:dd/MM/yyyy HH:mm}.", 409) { }
}

public sealed class ConsultaJaCanceladaException : DomainException
{
    public ConsultaJaCanceladaException(string id)
        : base("CONSULTA_JA_CANCELADA", $"A consulta '{id}' já está cancelada.", 409) { }
}

public sealed class DataConsultaNoPassadoException : DomainException
{
    public DataConsultaNoPassadoException()
        : base("DATA_NO_PASSADO", "A data da consulta deve ser futura.", 400) { }
}

public sealed class PacienteDaConsultaInexistenteException : DomainException
{
    public PacienteDaConsultaInexistenteException(string pacienteId)
        : base("PACIENTE_INEXISTENTE", $"Paciente '{pacienteId}' não existe.", 404) { }
}
