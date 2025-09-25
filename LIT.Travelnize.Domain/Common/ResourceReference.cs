namespace LIT.Travelnize.Domain.Common;

public sealed record ResourceReference : ValueObject
{
    public string Name { get; private init; } = string.Empty;
    public string Source { get; private init; } = string.Empty;
    public ResourceKind Kind { get; private init; }
    public string? Thumbnail { get; private init; }
    public bool IsExternal { get; private init; }

    private ResourceReference() { }

    public ResourceReference(string name, string source, ResourceKind kind, string? thumbnail, bool isExternal)
    {
        Name = name;
        Source = source;
        Kind = kind;
        Thumbnail = thumbnail;
        IsExternal = isExternal;
    }

    public static ResourceReference FromUpload(string originalFileName, string storedPath, string? thumbnail = null, ResourceKind? forceKind = null)
    {
        if (string.IsNullOrWhiteSpace(originalFileName)) throw new ArgumentException("Name leer", nameof(originalFileName));
        if (string.IsNullOrWhiteSpace(storedPath)) throw new ArgumentException("Pfad leer", nameof(originalFileName));

        originalFileName = originalFileName.Trim();
        storedPath = storedPath.Trim();

        var kind = forceKind ?? DetectKindFromPathOrUrl(storedPath, isExternal: false);
        return new ResourceReference(originalFileName, storedPath, kind, Normalize(thumbnail), isExternal: false);
    }

    public static ResourceReference FromUrl(string title, string url, string? thumbnail = null, ResourceKind? forceKind = null)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Titel leer", nameof(title));
        if (string.IsNullOrWhiteSpace(url)) throw new ArgumentException("URL leer", nameof(url));

        title = title.Trim();
        url = url.Trim();

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) ||
            uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
            throw new ArgumentException("Ungültige URL", nameof(url));

        var kind = forceKind ?? DetectKindFromUrl(uri);
        return new ResourceReference(title, url, kind, Normalize(thumbnail), isExternal: true);
    }

    public ResourceReference WithThumbnail(string? thumbnail)
        => this with { Thumbnail = Normalize(thumbnail) };

    public ResourceReference Rename(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName)) throw new ArgumentException("Name leer", nameof(newName));
        return this with { Name = newName.Trim() };
    }

    public ResourceReference VerifyKind(ResourceKind exprectedKind)
    {
        if (Kind != exprectedKind)
            throw new ArgumentException("Kind not allowed", nameof(exprectedKind));
        return this;
    }

    private static string? Normalize(string? v)
        => string.IsNullOrWhiteSpace(v) ? null : v.Trim();

    private static ResourceKind DetectKindFromUrl(Uri uri)
    {
        var direct = DetectKindFromPathOrUrl(uri.AbsoluteUri, isExternal: true);
        if (direct != ResourceKind.Website && direct != ResourceKind.Other)
            return direct;
        return ResourceKind.Website;
    }

    private static ResourceKind DetectKindFromPathOrUrl(string value, bool isExternal)
    {
        var lower = value.ToLowerInvariant();

        if (EndsWith(lower, ".png", ".jpg", ".jpeg", ".gif", ".webp", ".bmp", ".svg"))
            return ResourceKind.Image;
        if (EndsWith(lower, ".mp4", ".mov", ".webm", ".mkv", ".avi"))
            return ResourceKind.Video;
        if (EndsWith(lower, ".mp3", ".wav", ".ogg", ".m4a", ".flac"))
            return ResourceKind.Audio;
        if (EndsWith(lower, ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".txt", ".md"))
            return ResourceKind.Document;

        if (isExternal) return ResourceKind.Website;
        return ResourceKind.Other;
    }

    private static bool EndsWith(string value, params string[] suffixes)
    {
        if (suffixes == null || suffixes.Length == 0) return false;
        return suffixes.Any(suffix => value.EndsWith(suffix, StringComparison.OrdinalIgnoreCase));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
        yield return Source;
        yield return Kind;
        if (Thumbnail != null)
            yield return Thumbnail;
        yield return IsExternal;
    }
}