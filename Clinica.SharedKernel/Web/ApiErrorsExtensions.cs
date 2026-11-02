using Clinica.SharedKernel.Errors;
using Clinica.SharedKernel.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Clinica.SharedKernel.Web;

public static class ApiErrorsExtensions
{
    /// <summary>
    /// Faz os erros de validação de DTO (DataAnnotations) usarem o mesmo
    /// formato ErrorResponse dos erros de domínio, em vez do ProblemDetails padrão.
    /// </summary>
    public static IMvcBuilder AddPadraoDeErros(this IMvcBuilder builder)
    {
        builder.ConfigureApiBehaviorOptions(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var details = context.ModelState
                    .Where(e => e.Value is { Errors.Count: > 0 })
                    .ToDictionary(
                        e => e.Key,
                        e => e.Value!.Errors
                            .Select(x => string.IsNullOrWhiteSpace(x.ErrorMessage) ? "Valor inválido." : x.ErrorMessage)
                            .ToArray());

                var body = ErrorResponse.Create(
                    ErrorCodes.Validacao,
                    "Um ou mais campos são inválidos.",
                    StatusCodes.Status400BadRequest,
                    context.HttpContext.TraceIdentifier,
                    details);

                return new BadRequestObjectResult(body);
            };
        });

        return builder;
    }
}
