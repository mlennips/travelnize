namespace LIT.Travelnize.Domain.External.Wikipedia
{
    public sealed record WikipediaSummaryDto(
        string Title,
        string? ShortDescription,
        string? Extract,
        string? ThumbnailUrl,
        string? OriginalImageUrl,
        string? PageUrl,
        double? Latitude,
        double? Longitude
    );

}
