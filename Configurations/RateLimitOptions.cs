namespace HackerNewsApi.Configurations;

public class RateLimitOptions
{
    public int MaxRequests { get; set; }
    public int IntervalSeconds { get; set; }
    public int MaxQueue { get; set; }
    public int SemaphoreLimit { get; set; }
}