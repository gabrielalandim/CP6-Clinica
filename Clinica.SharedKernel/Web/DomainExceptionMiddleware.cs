using Clinica.SharedKernel.Errors;
using Clinica.SharedKernel.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Clinica.SharedKernel.Web;

/// <summary>
/// Rede de segurança global: qualquer DomainException que escape dos UseCases
/// vira um ErrorResponse com o status/código definidos na própria exceção.
/// Exceções inesperadas viram 500 sem vazar detalhes internos.
/// </summary>
public sealed class DomainExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<DomainExceptionMiddleware> _logger;

    public DomainExceptionMiddleware(RequestDelegate next, ILogger<DomainExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (DomainException ex) when (!context.Response.HasStarted)
        {
            _logger.LogWarning("Erro de domínio {Code}: {Message}", ex.Code, ex.Message);
            context.Response.StatusCode = ex.StatusCode;
            await context.Response.WriteAsJsonAsync(
                ErrorResponse.Create(ex.Code, ex.Message, ex.StatusCode, context.TraceIdentifier));
        }
        catch (Exception ex) when (!context.Response.HasStarted)
        {
            _logger.LogError(ex, "Erro não tratado");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(
                ErrorResponse.Create(ErrorCodes.ErroInterno, "Ocorreu um erro inesperado.",
                    StatusCodes.Status500InternalServerError, context.TraceIdentifier));
        }
    }
}

public static class DomainExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseDomainExceptionHandler(this IApplicationBuilder app) =>
        app.UseMiddleware<DomainExceptionMiddleware>();
}
