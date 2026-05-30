namespace FinaccApi.Shared.Kernel;

/// <summary>
/// Представляет результат выполнения операции без возвращаемого значения.
/// </summary>
public class Result
{
    /// <summary>
    /// Создает результат операции и проверяет согласованность статуса с ошибкой.
    /// </summary>
    /// <param name="isSuccess">Признак успешного выполнения операции.</param>
    /// <param name="error">Ошибка, связанная с неуспешным результатом.</param>
    /// <exception cref="InvalidOperationException">
    /// Выбрасывается, если успешный результат содержит ошибку или неуспешный результат не содержит ошибку.
    /// </exception>
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
    /// Возвращает true, если операция завершилась успешно.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Возвращает true, если операция завершилась с ошибкой.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Ошибка операции. Для успешного результата содержит <see cref="Error.None"/>.
    /// </summary>
    public Error Error { get; }

    /// <summary>
    /// Создает успешный результат без возвращаемого значения.
    /// </summary>
    /// <returns>Успешный результат операции.</returns>
    public static Result Success()
        => new(true, Error.None);

    /// <summary>
    /// Создает неуспешный результат с указанной ошибкой.
    /// </summary>
    /// <param name="error">Ошибка, описывающая причину неуспешного результата.</param>
    /// <returns>Неуспешный результат операции.</returns>
    public static Result Failure(Error error)
        => new(false, error);

    /// <summary>
    /// Создает успешный результат с возвращаемым значением.
    /// </summary>
    /// <typeparam name="TValue">Тип значения результата.</typeparam>
    /// <param name="value">Значение, полученное в результате операции.</param>
    /// <returns>Успешный результат операции со значением.</returns>
    public static Result<TValue> Success<TValue>(TValue value)
        => new(value);

    /// <summary>
    /// Создает неуспешный результат со значением указанного типа.
    /// </summary>
    /// <typeparam name="TValue">Тип значения результата.</typeparam>
    /// <param name="error">Ошибка, описывающая причину неуспешного результата.</param>
    /// <returns>Неуспешный результат операции без доступного значения.</returns>
    public static Result<TValue> Failure<TValue>(Error error)
        => new(error);
}


/// <summary>
/// Представляет результат выполнения операции с возвращаемым значением.
/// </summary>
/// <typeparam name="TValue">Тип значения результата.</typeparam>
public sealed class Result<TValue> : Result
{
    /// <summary>
    /// Значение, доступное только для успешного результата.
    /// </summary>
    private readonly TValue? _value;

    /// <summary>
    /// Создает успешный результат с указанным значением.
    /// </summary>
    /// <param name="value">Значение, полученное в результате операции.</param>
    internal Result(TValue value)
        : base(true, Error.None)
    {
        _value = value;
    }

    /// <summary>
    /// Создает неуспешный результат с указанной ошибкой.
    /// </summary>
    /// <param name="error">Ошибка, описывающая причину неуспешного результата.</param>
    internal Result(Error error)
        : base(false, error)
    {
        _value = default;
    }

    /// <summary>
    /// Значение успешного результата.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Выбрасывается при попытке получить значение неуспешного результата.
    /// </exception>
    public TValue Value
    {
        get
        {
            if (IsFailure)
                throw new InvalidOperationException("Cannot access value of a failed result.");

            return _value!;
        }
    }
}
