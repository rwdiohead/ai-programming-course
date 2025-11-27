namespace LegacyApi.Core;

/// <summary>
/// Objeto Result para encapsular respuestas de operaciones
/// </summary>
public class Result
{
    public bool Success { get; set; }
    public required string Message { get; set; }
    public object? Data { get; set; }

    public static Result Ok(string message = "Operación exitosa", object? data = null)
    {
        return new Result { Success = true, Message = message, Data = data };
    }

    public static Result Fail(string message)
    {
        return new Result { Success = false, Message = message, Data = null };
    }
}

/// <summary>
/// Versión genérica de Result
/// </summary>
public class Result<T>
{
    public bool Success { get; set; }
    public required string Message { get; set; }
    public T? Data { get; set; }

    public static Result<T> Ok(T data, string message = "Operación exitosa")
    {
        return new Result<T> { Success = true, Message = message, Data = data };
    }

    public static Result<T> Fail(string message)
    {
        return new Result<T> { Success = false, Message = message, Data = default };
    }
}
