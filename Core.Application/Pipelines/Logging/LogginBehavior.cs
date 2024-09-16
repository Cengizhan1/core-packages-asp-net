using System.Text.Json;
using Core.CrossCuttingConcerns.Logging;
using Core.CrossCuttingConcerns.Serilog;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Core.Application.Pipelines.Logging;

public class LogginBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, ILoggableRequest
{

    private readonly IHttpContextAccessor _contextAccessor;
    private readonly LoggerServiceBase _loggerServiceBase;

    public LogginBehavior(IHttpContextAccessor contextAccessor, LoggerServiceBase loggerServiceBase)
    {
        _contextAccessor = contextAccessor;
        _loggerServiceBase = loggerServiceBase;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        List<LogParameter> parameters = new()
        {
            new LogParameter{Type=request.GetType().Name, Value=request},
        };

        LogDetail logDetail = new()
        {
            
            MethodName = next.Method.Name,
            Parameters = parameters,   
            User = _contextAccessor.HttpContext?.User.Identity?.Name ?? "?"
        };
        _loggerServiceBase.Info(JsonSerializer.Serialize(logDetail));

        return await next();

    }
}