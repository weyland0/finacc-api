namespace finacc.Shared.Kernel;

/// <summary>
/// Результат операции с возвращаемым значением типа TValue.
/// </summary>
/// <typeparam name="TValue">Тип значения, возвращаемый результатом.</typeparam>
public sealed class Result<TValue> : Result
{
    /// <summary>
    /// Возвращаемое значение операции.
    /// </summary>
    private readonly TValue? _value;

    /// <summary>
    /// Конструктор результата.
    /// </summary>
    /// <param name="value">Возвращаемое значение.</param>
    internal Result(TValue value)
        : base(true, Error.None)
    {
        _value = value;
    }

    /// <summary>
    /// Конструктор результата.
    /// </summary>
    /// <param name="error">Ошибка.</param>
    internal Result(Error error)
        : base(false, error)
    {
        _value = default;
    }

    /// <summary>
    /// Свойство для доступа к возвращаемому значению, если результат успешен,
    /// иначе выбрасывает исключение InvalidOperationException.
    /// </summary>
    /// <returns>Возвращаемое значение операции.</returns>
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
