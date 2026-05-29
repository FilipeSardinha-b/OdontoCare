using System.Net;
using System.Text.Json;

namespace OdontoCare.Api.Middleware;

// Captura qualquer exceção não tratada e retorna resposta padronizada
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro não tratado: {Message}", ex.Message);
            await TratarExcecaoAsync(context, ex);
        }
    }

    private static Task TratarExcecaoAsync(HttpContext context, Exception ex)
    {
        var statusCode = ex switch
        {
            InvalidOperationException => HttpStatusCode.BadRequest,
            UnauthorizedAccessException => HttpStatusCode.Unauthorized,
            KeyNotFoundException => HttpStatusCode.NotFound,
            _ => HttpStatusCode.InternalServerError
        };

        var resposta = JsonSerializer.Serialize(new
        {
            mensagem = ex.Message,
            status = (int)statusCode
        });

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        return context.Response.WriteAsync(resposta);
    }
}