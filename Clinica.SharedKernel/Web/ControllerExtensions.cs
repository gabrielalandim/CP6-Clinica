using Clinica.SharedKernel.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Clinica.SharedKernel.Web;

public static class ControllerExtensions
{
    /// <summary>
    /// Converte o OperationResult vindo da camada de aplicação na resposta HTTP.
    /// Sucesso → status + payload. Falha → status + ErrorResponse padronizado.
    /// </summary>
    public static IActionResult ToActionResult<T>(this ControllerBase controller, OperationResult<T> result)
    {
        if (!result.IsSuccess)
        {
            var body = ErrorResponse.Create(
                result.ErrorCode ?? "ERRO",
                result.Error ?? "Falha ao processar a requisição.",
                result.StatusCode,
                controller.HttpContext.TraceIdentifier);

            return controller.StatusCode(result.StatusCode, body);
        }

        if (result.StatusCode == StatusCodes.Status204NoContent)
            return controller.NoContent();

        return controller.StatusCode(result.StatusCode, result.Value);
    }
}
