using Clinica.SharedKernel.Errors;

namespace Clinica.SharedKernel.Results;

public class OperationResult<T>
{
    public bool IsSuccess { get; private set; }
    public T? Value { get; private set; }
    public string? Error { get; private set; }
    public string? ErrorCode { get; private set; }
    public int StatusCode { get; private set; } = 200;

    public static OperationResult<T> Success(T value, int statusCode = 200) =>
        new() { IsSuccess = true, Value = value, StatusCode = statusCode };

    public static OperationResult<T> Failure(string error, int statusCode = 400, string? errorCode = null) =>
        new() { IsSuccess = false, Error = error, StatusCode = statusCode, ErrorCode = errorCode };

    public static OperationResult<T> FromException(DomainException ex) =>
        Failure(ex.Message, ex.StatusCode, ex.Code);
}
