using LIT.Travelnize.Domain.External.Wikipedia;
using Microsoft.Extensions.Caching.Memory;

namespace LIT.Travelnize.Api.External.Wikipedia;

public static class WikipediaEndpoints
{
    public static IEndpointRouteBuilder MapWikipediaEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes
            .MapGroup("/external/wikipedia")
            .WithTags("Wikipedia");

        group.MapGet("/summary",
            async Task<IResult> (string title, string? lang, IWikipediaClient client) =>
            {
                lang ??= "de";
                if (string.IsNullOrWhiteSpace(title))
                    return Results.BadRequest("title fehlt");

                var result = await client.GetSummaryAsync(title, lang);
                return result is null
                    ? Results.NotFound()
                    : Results.Ok(result);
            })
            .WithName("Wikipedia_GetSummary")
            .Produces<WikipediaSummaryDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces<string>(StatusCodes.Status400BadRequest);

        group.MapGet("/search",
            async Task<IResult> (string query, string? lang, int? limit, IWikipediaClient client) =>
            {
                lang ??= "de";
                var take = limit is < 1 or > 50 ? 10 : limit!.Value;
                if (string.IsNullOrWhiteSpace(query))
                    return Results.BadRequest("query fehlt");

                var results = await client.SearchAsync(query, lang, take);
                return Results.Ok(results);
            })
            .WithName("Wikipedia_Search")
            .Produces<IReadOnlyList<WikipediaSearchResultDto>>(StatusCodes.Status200OK)
            .Produces<string>(StatusCodes.Status400BadRequest);

        return routes;
    }
}

public interface IWikipediaClient
{
    Task<WikipediaSummaryDto?> GetSummaryAsync(string title, string lang);
    Task<IReadOnlyList<WikipediaSearchResultDto>> SearchAsync(string query, string lang, int limit);
}

public sealed class WikipediaClient : IWikipediaClient
{
    private readonly HttpClient _http;
    private readonly IMemoryCache _cache;
    private static readonly string CachePrefix = "wikisummary:";
    private static readonly string SearchCachePrefix = "wikisearch:";

    public WikipediaClient(HttpClient http, IMemoryCache cache)
    {
        _http = http;
#pragma warning disable S1075
        _http.DefaultRequestHeaders.UserAgent.ParseAdd("TravelnizeApp/1.0 (+https://example.com)");
#pragma warning restore S1075
        _cache = cache;
    }

    public async Task<WikipediaSummaryDto?> GetSummaryAsync(string title, string lang)
    {
        var key = $"{CachePrefix}{lang}:{title.ToLowerInvariant()}";
        if (_cache.TryGetValue(key, out WikipediaSummaryDto? cached) && cached is not null)
            return cached;

        var url = $"https://{lang}.wikipedia.org/api/rest_v1/page/summary/{Uri.EscapeDataString(title)}";
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(4));

        try
        {
            var resp = await _http.GetAsync(url, cts.Token);
            if (!resp.IsSuccessStatusCode)
                return null;

            var api = await resp.Content.ReadFromJsonAsync<WikipediaRestSummaryResponse>(cancellationToken: cts.Token);
            if (api == null || api.Type == "disambiguation")
                return null;

            var dto = new WikipediaSummaryDto(
                api.Title,
                api.Description,
                api.Extract,
                api.Thumbnail?.Source,
                api.OriginalImage?.Source,
                api.ContentUrls?.Desktop?.Page,
                api.Coordinates?.Lat,
                api.Coordinates?.Lon
            );

            _cache.Set(key, dto, TimeSpan.FromHours(12));
            return dto;
        }
        catch
        {
            return null;
        }
    }

    public async Task<IReadOnlyList<WikipediaSearchResultDto>> SearchAsync(string query, string lang, int limit)
    {
        var cacheKey = $"{SearchCachePrefix}{lang}:{limit}:{query.ToLowerInvariant()}";
        if (_cache.TryGetValue(cacheKey, out IReadOnlyList<WikipediaSearchResultDto>? cached) && cached is not null)
            return cached;

        var url = $"https://{lang}.wikipedia.org/w/rest.php/v1/search/title?q={Uri.EscapeDataString(query)}&limit={limit}&fields=description";
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(4));

        try
        {
            var resp = await _http.GetAsync(url, cts.Token);
            if (!resp.IsSuccessStatusCode)
                return Array.Empty<WikipediaSearchResultDto>();

            var api = await resp.Content.ReadFromJsonAsync<WikipediaSearchResponse>(cancellationToken: cts.Token);
            if (api?.Pages is null || api.Pages.Count == 0)
                return Array.Empty<WikipediaSearchResultDto>();

            var list = api.Pages
                .Select(p => new WikipediaSearchResultDto(
                    p.Title ?? p.Key ?? "",
                    p.Description,
                    p.Excerpt,
                    p.Key is null ? null : $"https://{lang}.wikipedia.org/wiki/{Uri.EscapeDataString(p.Key)}",
                    p.Thumbnail != null ? "https:" + p.Thumbnail?.Url : null
                ))
                .ToArray();

            _cache.Set(cacheKey, list, TimeSpan.FromHours(6));
            return list;
        }
        catch
        {
            return Array.Empty<WikipediaSearchResultDto>();
        }
    }

    private sealed class WikipediaRestSummaryResponse
    {
        public string? Type { get; set; } = default!;
        public string Title { get; set; } = default!;
        public string? Description { get; set; } = default!;
        public string? Extract { get; set; } = default!;
        public ThumbnailInfo? Thumbnail { get; set; } = default!;
        public ThumbnailInfo? OriginalImage { get; set; } = default!;
        public ContentUrlsInfo? ContentUrls { get; set; } = default!;
        public CoordInfo? Coordinates { get; set; } = default!;

        public sealed class ThumbnailInfo { public string? Source { get; set; } = default!; }
        public sealed class ContentUrlsInfo
        {
            public DesktopInfo? Desktop { get; set; } = default!;
            public sealed class DesktopInfo { public string? Page { get; set; } = default!; }
        }
        public sealed class CoordInfo { public double? Lat { get; set; } = default!; public double? Lon { get; set; } = default!; }
    }

    private sealed class WikipediaSearchResponse
    {
        public List<Page>? Pages { get; set; } = default!;
        public sealed class Page
        {
            public int Id { get; set; } = default!;
            public string? Key { get; set; } = default!;
            public string? Title { get; set; } = default!;
            public string? Excerpt { get; set; } = default!;
            public string? Description { get; set; } = default!;
            public Thumb? Thumbnail { get; set; } = default!;
        }
        public sealed class Thumb { public string? Url { get; set; } = default!; }
    }
}