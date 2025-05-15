using HackerNewsApi.DTOs;
using HackerNewsApi.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HackerNewsApi.Controllers;

[ApiController]
[Route("api/v1/hackernews")]
public class HackerNewsController : ControllerBase
{
    private readonly IHackerNewsService _hackerNewsService;

    public HackerNewsController(IHackerNewsService hackerNewsService)
    {
        _hackerNewsService = hackerNewsService;
    }

    [HttpGet("beststories")]
    public async Task<ActionResult<List<StoryDto>>> GetBestStories([FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var stories = await _hackerNewsService.GetBestStoriesAsync(pageSize, cancellationToken);
        return Ok(stories);
    }
}