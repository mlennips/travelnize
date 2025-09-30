using System.Net;
using System.Net.Http.Json;
using LIT.Travelnize.Domain.External.Wikipedia;
using LIT.Travelnize.Interfaces;
using Microsoft.AspNetCore.WebUtilities;

namespace LIT.Travelnize.Services.External;

public sealed class WikipediaApiClient(HttpClient http) : IWikipediaApiClient
{
    private static string BasePath => "external/wikipedia";

    public async Task<WikipediaSummaryDto?> GetSummaryAsync(string title, string? lang = null, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("title darf nicht leer sein", nameof(title));

        var url = QueryHelpers.AddQueryString($"{BasePath}/summary", new Dictionary<string, string?>
        {
            ["title"] = title,
            ["lang"] = lang
        });

        using var resp = await http.GetAsync(url, ct);
        if (resp.StatusCode == HttpStatusCode.NotFound)
            return null;
        if (!resp.IsSuccessStatusCode)
            return null;

        return await resp.Content.ReadFromJsonAsync<WikipediaSummaryDto>(cancellationToken: ct);
    }

    public async Task<IReadOnlyList<WikipediaSearchResultDto>> SearchAsync(string query, string? lang = null, int limit = 10, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(query))
            return Array.Empty<WikipediaSearchResultDto>();

        if (limit is < 1 or > 50)
            limit = 10;

        var url = QueryHelpers.AddQueryString($"{BasePath}/search", new Dictionary<string, string?>
        {
            ["query"] = query,
            ["lang"] = lang,
            ["limit"] = limit.ToString()
        });

        using var resp = await http.GetAsync(url, ct);
        if (!resp.IsSuccessStatusCode)
            return Array.Empty<WikipediaSearchResultDto>();

        var data = await resp.Content.ReadFromJsonAsync<IReadOnlyList<WikipediaSearchResultDto>>(cancellationToken: ct);
        return data ?? Array.Empty<WikipediaSearchResultDto>();
    }
}