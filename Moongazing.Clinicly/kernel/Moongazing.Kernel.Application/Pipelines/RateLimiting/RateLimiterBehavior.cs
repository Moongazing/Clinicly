using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moongazing.Kernel.CrossCuttingConcerns.Exceptions.Types;

namespace Moongazing.Kernel.Application.Pipelines.RateLimiting;

public class RateLimiterBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, IRateLimitedRequest
{
    private readonly IMemoryCache memoryCache;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly RateLimitSettings settings;

    public RateLimiterBehavior(
        IMemoryCache memoryCache,
        IHttpContextAccessor httpContextAccessor,
        IOptions<RateLimitSettings> options)
    {
        this.memoryCache = memoryCache;
        this.httpContextAccessor = httpContextAccessor;
        settings = options.Value;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        string ip = httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "unknown";
        string cacheKey = $"ratelimit:{ip}:{typeof(TRequest).Name}";

        Console.WriteLine($"[RateLimit] IP: {ip}, Request: {typeof(TRequest).Name}");
        var counter = memoryCache.GetOrCreate(cacheKey, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(settings.PeriodInSeconds);
            return new RateLimitCounter { Count = 0 };
        });

        if (counter!.Count >= settings.Limit)
            throw new RateLimitRejectedException($"Rate limit exceeded: {settings.Limit} requests per {settings.PeriodInSeconds} seconds.");

        counter.Count++;
        memoryCache.Set(cacheKey, counter);

        return await next(cancellationToken);
    }

    private class RateLimitCounter
    {
        public int Count { get; set; }
    }
}

