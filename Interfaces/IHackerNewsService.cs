using HackerNewsApi.DTOs;

namespace HackerNewsApi.Interfaces;

public interface IHackerNewsService
{
    Task<List<StoryDto>> GetBestStoriesAsync(int pageSize, CancellationToken cancellationToken);
}