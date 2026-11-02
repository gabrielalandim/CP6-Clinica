using Clinica.SharedKernel.Errors;

namespace Clinica.Modules.Identity.Domain;

public sealed class CredenciaisInvalidasException : DomainException
{
    public CredenciaisInvalidasException()
        : base("CREDENCIAIS_INVALIDAS", "E-mail ou senha inválidos.", 401) { }
}

public sealed class UsuarioJaExisteException : DomainException
{
    public UsuarioJaExisteException(string email)
        : base("USUARIO_JA_EXISTE", $"Já existe um usuário com o e-mail '{email}'.", 409) { }
}
