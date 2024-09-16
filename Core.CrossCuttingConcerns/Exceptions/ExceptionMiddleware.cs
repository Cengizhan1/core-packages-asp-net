using System.Text.Json;
using Core.CrossCuttingConcerns.Exceptions.Handlers;
using Core.CrossCuttingConcerns.Logging;
using Core.CrossCuttingConcerns.Serilog;
using Microsoft.AspNetCore.Http;

namespace Core.CrossCuttingConcerns.Exceptions;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly HttpExceptionHandler _httpExceptionHandler;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly LoggerServiceBase _loggerService;

    public ExceptionMiddleware(RequestDelegate next, LoggerServiceBase loggerService, IHttpContextAccessor contextAccessor)
    {
        _next = next;
        _httpExceptionHandler = new HttpExceptionHandler();
        _loggerService = loggerService;
        _contextAccessor = contextAccessor;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await LogException(context, ex);
            await HandleExceptionAsync(context.Response, ex);
        }
    }

    private Task LogException(HttpContext context, Exception ex)
    {
        List<LogParameter> parameters = new()
        {
            new LogParameter{Type = context.GetType().Name, Value = ex.ToString() }
        };
        LogDetailWithException logDetail = new()
        {
            ExceptionMessage = ex.Message,
            MethodName = _next.Method.Name,
            Parameters = parameters,
            User = _contextAccessor.HttpContext?.User.Identity?.Name ?? "?"
        };

        _loggerService.Error(JsonSerializer.Serialize(logDetail));

        return Task.CompletedTask;
    }

    private Task HandleExceptionAsync(HttpResponse response, Exception ex)
    {
        response.ContentType = "application/json";
        _httpExceptionHandler.Response = response;
        return _httpExceptionHandler.HandleExpcetionAsync(ex);
    }
}
