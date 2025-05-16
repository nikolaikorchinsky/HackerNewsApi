using HackerNewsApi.Configurations;
using HackerNewsApi.Extentions;
using HackerNewsApi.Infrastructure;
using HackerNewsApi.Interfaces;
using HackerNewsApi.Middleware;
using HackerNewsApi.Services;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.OpenApi.Models;
using Polly;
using Polly.RateLimit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<RequestLoggingMiddleware>();

builder.Services.Configure<HackerNewsOptions>(builder.Configuration.GetSection("HackerNewsApi"));
builder.Services.Configure<CacheOptions>(builder.Configuration.GetSection("CacheSettings"));
builder.Services.Configure<RateLimitOptions>(builder.Configuration.GetSection("RateLimit"));
builder.Services.AddOptions<RateLimitOptions>()
    .Validate(options => options.SemaphoreLimit > 0, "SemaphoreLimit must be greater than 0");

builder.Services.AddMemoryCache();

var rateLimitOptions = builder.Configuration.GetSection("RateLimit").Get<RateLimitOptions>();
var rateLimitPolicy = Policy.RateLimitAsync(rateLimitOptions.MaxRequests, TimeSpan.FromSeconds(rateLimitOptions.IntervalSeconds), rateLimitOptions.MaxQueue);

var retryPolicy = Policy
    .Handle<RateLimitRejectedException>()
    .WaitAndRetryAsync(
        retryCount: 5, 
        sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
        onRetry: (exception, timeSpan, retryCount, context) =>
        {
            Console.WriteLine($"Rate limit hit. Retrying in {timeSpan.TotalSeconds} seconds... (Retry {retryCount})");
        }
    );

var combinedPolicy = Policy.WrapAsync(retryPolicy, rateLimitPolicy);

builder.Services.AddHttpClient<IHackerNewsClient, HackerNewsClient>()
    .AddHttpMessageHandler(() => new RateLimitingHandler(combinedPolicy));

builder.Services.AddScoped<IHackerNewsService, HackerNewsService>();
builder.Services.AddControllers();

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "HackerNews API",
        Version = "v1",
        Description = "API for retrieving top stories from Hacker News",
    });
});

var app = builder.Build();

app.UseMiddleware<RequestLoggingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "HackerNews API v1");
        c.RoutePrefix = "";
    });
}

app.UseRouting();
app.UseEndpoints(endpoints => endpoints.MapControllers());

app.Run();

