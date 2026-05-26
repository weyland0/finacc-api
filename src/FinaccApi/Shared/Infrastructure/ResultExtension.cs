using Microsoft.AspNetCore.Mvc;
using FinaccApi.Shared.Kernel;

namespace FinaccApi.Shared.Infrastructure;

/// <summary>
/// Расширения для преобразования Result в IActionResult для использования в ASP.NET Core контроллерах.
/// </summary>
public static class ResultExtension
{
    /// <summary>
    /// Преобразует Result<T> в IActionResult.
    /// При успехе возвращает чистый DTO (200 OK).
    /// При ошибке возвращает ProblemDetails с соответствующим статус-кодом.
    /// </summary>
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
    /// Преобразует Result в IActionResult.
    /// При успехе возвращает 200.
    /// При ошибке возвращает ProblemDetails с соответствующим статус-кодом.
    /// </summary>
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
    /// Получает URI типа проблемы на основе HTTP статус-кода.
    /// </summary>
    /// <param name="statusCode"> HTTP статус-код </param>
    /// <returns> URI типа проблемы </returns>
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
    /// Получает HTTP статус-код на основе типа ошибки.
    /// </summary>
    /// <param name="errorType"> Тип ошибки, используемый внутри API </param>
    /// <returns> HTTP статус-код </returns>
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
    /// Преобразует внутреннюю ошибку в ProblemDetails для возвращения клиенту.
    /// </summary>
    /// <param name="error"> Внутренняя ошибка </param>
    /// <returns> ProblemDetails </returns>
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
