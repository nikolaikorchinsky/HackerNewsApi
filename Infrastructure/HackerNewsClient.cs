using HackerNewsApi.Configurations;
using HackerNewsApi.Interfaces;
using HackerNewsApi.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly.RateLimit;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace HackerNewsApi.Infrastructure;
public class HackerNewsClient : IHackerNewsClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<HackerNewsClient> _logger;
    private readonly HackerNewsOptions _options;

    public HackerNewsClient(HttpClient httpClient, IOptions<HackerNewsOptions> options, ILogger<HackerNewsClient> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<List<int>?> GetBestStoryIdsAsync(CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_options.BaseUrl}{_options.BestStoriesEndpoint}", cancellationToken);

            response.EnsureSuccessStatusCode(); // Проверяем успешный статус ответа

            return await response.Content.ReadFromJsonAsync<List<int>>(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Request for story IDs was canceled by the user.");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error occurred while retrieving story IDs.");
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON deserialization error occurred while retrieving story IDs.");
        }
        catch (TimeoutException ex)
        {
            _logger.LogError(ex, "Timeout occurred while retrieving story IDs.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unknown error occurred while retrieving story IDs.");
        }

        return null;
    }

    public async Task<Story?> GetStoryByIdAsync(int id, CancellationToken cancellationToken)
    {
        try
        {
            var url = $"{_options.BaseUrl}{string.Format(_options.ItemEndpoint, id)}";
            var response = await _httpClient.GetAsync(url, cancellationToken);

            response.EnsureSuccessStatusCode(); // Проверяем, что статус ответа 200-299

            return await response.Content.ReadFromJsonAsync<Story>(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Request for story IDs was canceled by the user.");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error occurred while retrieving story IDs.");
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON deserialization error occurred while retrieving story IDs.");
        }
        catch (TimeoutException ex)
        {
            _logger.LogError(ex, "Timeout occurred while retrieving story IDs.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unknown error occurred while retrieving story IDs.");
        }

        return null;
    }
}

