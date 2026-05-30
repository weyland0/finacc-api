using Microsoft.AspNetCore.Mvc;
using FinaccApi.Shared.Kernel;

namespace FinaccApi.Shared.Infrastructure;

/// <summary>
/// Содержит методы преобразования результатов приложения в HTTP-ответы ASP.NET Core.
/// </summary>
public static class ResultExtension
{
    /// <summary>
    /// Преобразует результат со значением в HTTP-ответ.
    /// </summary>
    /// <typeparam name="T">Тип значения успешного результата.</typeparam>
    /// <param name="result">Результат операции приложения.</param>
    /// <returns>
    /// HTTP 200 со значением при успехе или <see cref="ProblemDetails"/> с подходящим статусом при ошибке.
    /// </returns>
    public static IActionResult ToActionResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
        {
            return new OkObjectResult(result.Value);
        }

        var problemDetails = GetProblemDetails(result.Error);
        return new ObjectResult(problemDetails) { StatusCode = problemDetails.Status };
    }

    /// <summary>
    /// Преобразует результат без значения в HTTP-ответ.
    /// </summary>
    /// <param name="result">Результат операции приложения.</param>
    /// <returns>
    /// HTTP 200 без тела при успехе или <see cref="ProblemDetails"/> с подходящим статусом при ошибке.
    /// </returns>
    public static IActionResult ToActionResult(this Result result)
    {
        if (result.IsSuccess) 
        {
            return new OkResult();
        }

        var problemDetails = GetProblemDetails(result.Error);
        return new ObjectResult(problemDetails) { StatusCode = problemDetails.Status };
    }

    /// <summary>
    /// Возвращает URI спецификации HTTP для указанного статус-кода ошибки.
    /// </summary>
    /// <param name="statusCode">HTTP-статус ошибки.</param>
    /// <returns>URI типа проблемы для <see cref="ProblemDetails"/>.</returns>
    private static string GetProblemType(int statusCode)
    {
        return statusCode switch
        {
            400 => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            401 => "https://tools.ietf.org/html/rfc7235#section-3.1",
            403 => "https://tools.ietf.org/html/rfc7231#section-6.5.3",
            404 => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            409 => "https://tools.ietf.org/html/rfc7231#section-6.5.8",
            500 => "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            _ =>  "https://tools.ietf.org/html/rfc7231#section-6.6.1"
        };
    }

    /// <summary>
    /// Сопоставляет тип ошибки приложения с HTTP-статусом.
    /// </summary>
    /// <param name="errorType">Тип ошибки, используемый внутри API.</param>
    /// <returns>HTTP-статус, соответствующий типу ошибки.</returns>
    private static int GetStatusCode(ErrorType errorType)
    {
        return errorType switch
        {
            ErrorType.Validation => 400,
            ErrorType.Unauthorized => 401,
            ErrorType.Forbidden => 403,
            ErrorType.NotFound => 404,
            ErrorType.Conflict => 409,
            ErrorType.Failure => 500,
            _ => 500
        };
    }

    /// <summary>
    /// Преобразует ошибку приложения в объект <see cref="ProblemDetails"/> для ответа клиенту.
    /// </summary>
    /// <param name="error">Ошибка приложения.</param>
    /// <returns>Описание проблемы в формате ASP.NET Core.</returns>
    private static ProblemDetails GetProblemDetails(Error error)
    {
        var statusCode = GetStatusCode(error.Type);
        var problemType = GetProblemType(statusCode);

        var problemDetails = new ProblemDetails
        {
            Title = error.Code,
            Detail = error.Message,
            Status =  statusCode,
            Type = problemType
        };

        return problemDetails;
    }
}
