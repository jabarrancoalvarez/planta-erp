namespace PlanTA.SharedKernel;

public class Result<T>
{
    private Result(bool isSuccess, T? value, Error? error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public T? Value { get; }
    public Error? Error { get; }

    public static Result<T> Success(T value) => new(true, value, null);
    public static Result<T> Failure(Error error) => new(false, default, error);
    public static Result<T> Failure(string code, string message) =>
        new(false, default, Error.Validation(code, message));
}

public record Error(string Code, string Message, ErrorType Type)
{
    public static Error NotFound(string code, string message) =>
        new(code, message, ErrorType.NotFound);

    public static Error Validation(string code, string message) =>
        new(code, message, ErrorType.Validation);

    public static Error Conflict(string code, string message) =>
        new(code, message, ErrorType.Conflict);

    public static Error Forbidden(string code, string message) =>
        new(code, message, ErrorType.Forbidden);

    public static Error Internal(string code, string message) =>
        new(code, message, ErrorType.Internal);
}

public enum ErrorType
{
    Validation,
    NotFound,
    Conflict,
    Forbidden,
    Internal
}
