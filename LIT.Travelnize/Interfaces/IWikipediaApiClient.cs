using LIT.Travelnize.Domain.External.Wikipedia;

namespace LIT.Travelnize.Interfaces;

public interface IWikipediaApiClient
{
    Task<WikipediaSummaryDto?> GetSummaryAsync(string title, string? lang = null, CancellationToken ct = default);
    Task<IReadOnlyList<WikipediaSearchResultDto>> SearchAsync(string query, string? lang = null, int limit = 10, CancellationToken ct = default);
}
