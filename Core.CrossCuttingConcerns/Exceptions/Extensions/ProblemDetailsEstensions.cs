using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Core.CrossCuttingConcerns.Exceptions.Extensions;

public static class ProblemDetailsEstensions
{
    public static string AsJson<IProblemDetail>(this IProblemDetail details)
        where IProblemDetail : ProblemDetails => JsonSerializer.Serialize(details);
}
