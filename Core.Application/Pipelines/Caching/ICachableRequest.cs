namespace Core.Application.Pipelines.Caching;

public interface ICachableRequest
{
    string CacheKey { get; }
    bool ByPass { get; }
    string? CacheGroupKey { get; }
    TimeSpan? SlidingExpiration { get; }
}
