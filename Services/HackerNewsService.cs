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
    private readonly IMapper _mapper;

    public HackerNewsService(
        IHackerNewsClient client,
        IMemoryCache memoryCache,
        IOptions<CacheOptions> cacheOptions,
        IMapper mapper)
    {
        _client = client;
        _memoryCache = memoryCache;
        _cacheOptions = cacheOptions.Value;
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

        var stories = new List<Story>();
        foreach (var id in storyIds)
        {
            var story = await _client.GetStoryByIdAsync(id, cancellationToken);
            if (story != null)
            {
                stories.Add(story);
            }
        }

        var sortedStories = stories.OrderByDescending(s => s.Score).ToList();
        _memoryCache.Set(CacheKey, sortedStories, TimeSpan.FromMinutes(_cacheOptions.ExpirationMinutes));
        
        return _mapper.Map<List<StoryDto>>(sortedStories.Take(pageSize));
    }
}