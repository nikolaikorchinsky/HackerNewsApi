using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Polly;

namespace HackerNewsApi.Extentions;

public class RateLimitingHandler : DelegatingHandler
{
    private readonly IAsyncPolicy _rateLimitPolicy;

    public RateLimitingHandler(IAsyncPolicy rateLimitPolicy)
    {
        _rateLimitPolicy = rateLimitPolicy;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return await _rateLimitPolicy.ExecuteAsync(() => base.SendAsync(request, cancellationToken));
    }
}