using System.Text.Json.Serialization;

namespace Clinica.SharedKernel.Results;

/// <summary>
/// Corpo padrão de TODA resposta de erro da API (erros de domínio, validação,
/// autenticação, autorização e erros inesperados).
/// </summary>
public sealed class ErrorResponse
{
    public string Code { get; init; } = string.Empty;
    public string Error { get; init; } = string.Empty;
    public int Status { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IDictionary<string, string[]>? Details { get; init; }

    public string? TraceId { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    public static ErrorResponse Create(
        string code,
        string error,
        int status,
        string? traceId = null,
        IDictionary<string, string[]>? details = null) =>
        new()
        {
            Code = code,
            Error = error,
            Status = status,
            TraceId = traceId,
            Details = details
        };
}
