using LIT.Travelnize;
using LIT.Travelnize.Interfaces;
using LIT.Travelnize.Services;
using LIT.Travelnize.Services.Api;
using LIT.Travelnize.Services.Auth;
using LIT.Travelnize.Services.External;
using LIT.Travelnize.States;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        builder.Services.AddScoped<IAuthService, JwtAuthenticationStateProvider>();
        builder.Services.AddScoped<AuthenticationStateProvider>(p => (JwtAuthenticationStateProvider)p.GetRequiredService<IAuthService>());
        builder.Services.AddScoped<JwtAuthorizationMessageHandler>();
        builder.Services.AddScoped<IAccessTokenService, AccessTokenService>();

        builder.Services.AddHttpClient<ApiClient>((sp, c) => c.BaseAddress = ResolveBackendBaseAddress(sp));
        builder.Services.AddHttpClient<AuthApiClient>((sp, c) => c.BaseAddress = ResolveBackendBaseAddress(sp));
        builder.Services.AddHttpClient<TripsApiClient>((sp, c) => c.BaseAddress = ResolveBackendBaseAddress(sp))
            .AddHttpMessageHandler<JwtAuthorizationMessageHandler>();

        builder.Services.AddAuthorizationCore();
        builder.Services.AddScoped<LocalStorageService>();
        builder.Services.AddMudServices();
        builder.Services.AddScoped<ScrollService>();
        builder.Services.AddScoped<IWikipediaApiClient, WikipediaApiClient>();
        builder.Services.AddSingleton<TripsState>();

        await builder.Build().RunAsync();
    }

    private static Uri ResolveBackendBaseAddress(IServiceProvider sp)
    {
        var config = sp.GetRequiredService<IConfiguration>();
        var nav = sp.GetRequiredService<NavigationManager>();
        var env = sp.GetRequiredService<IWebAssemblyHostEnvironment>();

        var raw = config["Backend:BaseUrl"];

        if (string.IsNullOrWhiteSpace(raw))
        {
            raw = env.IsDevelopment() ? "http://localhost:5005/" : "/api/";
        }

        var baseUri = nav.BaseUri; // z.B. http://localhost:8081/ oder file:///
        var isFileOrigin = baseUri.StartsWith("file://", StringComparison.OrdinalIgnoreCase);

        // Wenn per file:// geöffnet → relative /api nicht nutzbar → erzwungen absolute URL
        if (isFileOrigin && raw.StartsWith("/"))
        {
            // Debug-Fallback (anpassen falls andere Dev-URL)
            raw = "http://localhost:8080/api/";
        }

        static Uri EnsureTrailingSlash(Uri u) =>
            u.AbsoluteUri.EndsWith('/') ? u : new Uri(u.AbsoluteUri + "/");

        if (Uri.TryCreate(raw, UriKind.Absolute, out var abs))
        {
            Console.WriteLine($"[BackendBase] (abs) {abs}");
            return EnsureTrailingSlash(abs);
        }

        // Relativ → an Origin anhängen
        var origin = new Uri(baseUri);
        var combined = new Uri(origin, raw.TrimStart('/'));
        combined = EnsureTrailingSlash(combined);
        Console.WriteLine($"[BackendBase] (rel) {combined}");
        return combined;
    }
}