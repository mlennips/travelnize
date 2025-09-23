namespace LIT.Travelnize.Domain.External.Wikipedia
{
    public sealed record WikipediaSearchResultDto(
        string Title,
        string? Description,
        string? Excerpt,
        string? PageUrl,
        string? ThumbnailUrl
    );
}
