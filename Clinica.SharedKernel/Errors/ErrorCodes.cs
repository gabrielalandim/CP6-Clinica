namespace Clinica.SharedKernel.Errors;

/// <summary>
/// Códigos de erro transversais (não pertencem a nenhum módulo específico).
/// Os códigos de negócio ficam nas exceções de cada módulo.
/// </summary>
public static class ErrorCodes
{
    public const string Validacao = "VALIDACAO";
    public const string NaoAutenticado = "NAO_AUTENTICADO";
    public const string AcessoNegado = "ACESSO_NEGADO";
    public const string ErroInterno = "ERRO_INTERNO";
}
