using Microsoft.JSInterop;

namespace LIT.Travelnize.Services;

public sealed class ScrollService(IJSRuntime js) : IAsyncDisposable
{
    private readonly IJSRuntime _js = js;
    private IJSObjectReference? _module;
    private ValueTask<IJSObjectReference> EnsureModuleAsync() =>
        _module is not null
            ? ValueTask.FromResult(_module)
            : LoadAsync();

    private async ValueTask<IJSObjectReference> LoadAsync()
    {
        _module = await _js.InvokeAsync<IJSObjectReference>("import", "/js/interop/scrollHelpers.js");
        return _module;
    }

    public async Task ScrollToIdAsync(string id, ScrollOptions? options = null)
    {
        if (string.IsNullOrWhiteSpace(id)) return;
        var module = await EnsureModuleAsync();
        await module.InvokeVoidAsync("scrollToId", id, options);
    }

    public async ValueTask DisposeAsync()
    {
        if (_module is not null)
        {
            try { await _module.DisposeAsync(); } catch { }
        }
    }
}

public sealed record ScrollOptions(int? MaxTries = null, bool? Highlight = null, string? Behavior = null);