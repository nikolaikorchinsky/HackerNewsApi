using AutoMapper;
using HackerNewsApi.Configurations;
using HackerNewsApi.DTOs;
using HackerNewsApi.Interfaces;
using HackerNewsApi.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace HackerNewsApi.Services;

public class HackerNewsService : IHackerNewsService
{
    private const string CacheKey = "bestStories";
    private readonly IHackerNewsClient _client;
    private readonly IMemoryCache _memoryCache;
    private readonly CacheOptions _cacheOptions;
    private readonly RateLimitOptions _rateLimitOptions;
    private readonly IMapper _mapper;

    public HackerNewsService(
        IHackerNewsClient client,
        IMemoryCache memoryCache,
        IOptions<CacheOptions> cacheOptions,
        IOptions<RateLimitOptions> rateLimitOptions,
        IMapper mapper)
    {
        _client = client;
        _memoryCache = memoryCache;
        _cacheOptions = cacheOptions.Value;
        _rateLimitOptions = rateLimitOptions.Value;
        _mapper = mapper;
    }

    public async Task<List<StoryDto>> GetBestStoriesAsync(int pageSize, CancellationToken cancellationToken)
    {
        if (_memoryCache.TryGetValue(CacheKey, out List<Story> cachedStories))
        {
            return _mapper.Map<List<StoryDto>>(cachedStories.Take(pageSize));
        }

        var storyIds = await _client.GetBestStoryIdsAsync(cancellationToken);
        if (storyIds is null)
        {
            return new List<StoryDto>();
        }

        var semaphore = new SemaphoreSlim(_rateLimitOptions.SemaphoreLimit);
        var tasks = storyIds.Select(async id =>
        {
            await semaphore.WaitAsync(cancellationToken);
            try
            {
                return await _client.GetStoryByIdAsync(id, cancellationToken);
            }
            finally
            {
                semaphore.Release();
            }
        });

        var stories = (await Task.WhenAll(tasks))
            .Where(story => story != null)
            .ToList();

        var sortedStories = stories.OrderByDescending(s => s?.Score ?? 0).ToList();
        _memoryCache.Set(CacheKey, sortedStories, TimeSpan.FromMinutes(_cacheOptions.ExpirationMinutes));
        
        return _mapper.Map<List<StoryDto>>(sortedStories.Take(pageSize));
    }
}