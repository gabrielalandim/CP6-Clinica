using Clinica.SharedKernel.Errors;

namespace Clinica.Modules.Pacientes.Domain;

public sealed class PacienteNaoEncontradoException : DomainException
{
    public PacienteNaoEncontradoException(string id)
        : base("PACIENTE_NAO_ENCONTRADO", $"Paciente '{id}' não encontrado.", 404) { }
}

public sealed class PacienteEmailJaCadastradoException : DomainException
{
    public PacienteEmailJaCadastradoException(string email)
        : base("PACIENTE_EMAIL_DUPLICADO", $"Já existe um paciente com o e-mail '{email}'.", 409) { }
}

public sealed class PacienteComConsultasAgendadasException : DomainException
{
    public PacienteComConsultasAgendadasException(string id)
        : base("PACIENTE_COM_CONSULTAS_AGENDADAS",
               $"O paciente '{id}' possui consultas agendadas. Cancele-as antes de excluir o paciente.", 409) { }
}

public sealed class PacienteIdInvalidoException : DomainException
{
    public PacienteIdInvalidoException(string id)
        : base("PACIENTE_ID_INVALIDO", $"O id '{id}' não é um identificador válido.", 400) { }
}
