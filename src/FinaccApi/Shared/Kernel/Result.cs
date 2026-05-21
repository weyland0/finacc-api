namespace FinaccApi.Shared.Kernel;

/// <summary>
/// Результат операции.
/// </summary>
public class Result
{
    /// <summary>
    /// Конструктор результата.
    /// </summary>
    /// <param name="isSuccess">Успешный ли результат.</param>
    /// <param name="error">Ошибка.</param>
    protected Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None)
            throw new InvalidOperationException("Successful result cannot contain an error.");

        if (!isSuccess && error == Error.None)
            throw new InvalidOperationException("Failed result must contain an error.");

        IsSuccess = isSuccess;
        Error = error;
    }

    /// <summary>
    /// Успешный ли результат.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Неуспешный ли результат.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Ошибка, если результат неуспешный.
    /// </summary>
    public Error Error { get; }

    /// <summary>
    /// Создает успешный результат.
    /// </summary>
    /// <returns>Успешный результат.</returns>
    public static Result Success()
        => new(true, Error.None);

    /// <summary>
    /// Создает неуспешный результат.
    /// </summary>
    /// <param name="error">Ошибка.</param>
    /// <returns>Неуспешный результат.</returns>
    public static Result Failure(Error error)
        => new(false, error);

    /// <summary>
    /// Создает успешный результат с возвращаемым значением.
    /// </summary>
    /// <typeparam name="TValue">Тип значения, возвращаемый результатом.</typeparam>
    /// <param name="value">Возвращаемое значение.</param>
    /// <returns>Успешный результат с возвращаемым значением.</returns>
    public static Result<TValue> Success<TValue>(TValue value)
        => new(value);

    /// <summary>
    /// Создает неуспешный результат. 
    /// Возвращемое значение недоступно, так как результат неуспешный.
    /// </summary>
    /// <typeparam name="TValue">Тип значения, возвращаемый результатом.</typeparam>
    /// <param name="error">Ошибка.</param>
    /// <returns>Неуспешный результат.</returns>
    public static Result<TValue> Failure<TValue>(Error error)
        => new(error);
}
