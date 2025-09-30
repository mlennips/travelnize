using LIT.Travelnize;
using LIT.Travelnize.Interfaces;
using LIT.Travelnize.Services;
using LIT.Travelnize.Services.Api;
using LIT.Travelnize.Services.Auth;
using LIT.Travelnize.Services.External;
using LIT.Travelnize.States;
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

        var resolvedBackendBase = ResolveBackendBaseAddress(
            builder.Configuration,
            builder.HostEnvironment.BaseAddress,
            builder.HostEnvironment.IsDevelopment());

        builder.Services.AddSingleton(resolvedBackendBase);

        builder.Services.AddScoped<IAuthService, JwtAuthenticationStateProvider>();
        builder.Services.AddScoped<AuthenticationStateProvider>(p => (JwtAuthenticationStateProvider)p.GetRequiredService<IAuthService>());
        builder.Services.AddScoped<JwtAuthorizationMessageHandler>();
        builder.Services.AddScoped<IAccessTokenService, AccessTokenService>();

        builder.Services.AddHttpClient<ApiClient>((sp, c) =>
        {
            c.BaseAddress = sp.GetRequiredService<Uri>();
        });

        builder.Services.AddHttpClient<AuthApiClient>((sp, c) =>
        {
            c.BaseAddress = sp.GetRequiredService<Uri>();
        });

        builder.Services.AddHttpClient<TripsApiClient>((sp, c) =>
        {
            c.BaseAddress = sp.GetRequiredService<Uri>();
        }).AddHttpMessageHandler<JwtAuthorizationMessageHandler>();

        builder.Services.AddAuthorizationCore();
        builder.Services.AddScoped<LocalStorageService>();
        builder.Services.AddMudServices();
        builder.Services.AddScoped<ScrollService>();
        builder.Services.AddScoped<IWikipediaApiClient, WikipediaApiClient>();
        builder.Services.AddSingleton<TripsState>();

        Console.WriteLine($"[BackendBase] resolved = {resolvedBackendBase}");

        await builder.Build().RunAsync();
    }

    private static Uri ResolveBackendBaseAddress(WebAssemblyHostConfiguration config, string hostEnvironmentBaseAddress, bool isDev)
    {
        // Optional Konfiguration (z.B. wwwroot/appsettings.Development.json):
        // "Backend": { "BaseUrl": "https://localhost:5005/api/" }
        var configured = config["Backend:BaseUrl"]?.Trim();

        string EnsureTrailingSlash(string v) => v.EndsWith('/') ? v : v + "/";

        Uri MakeAbsolute(string value)
        {
            value = EnsureTrailingSlash(value);
            return new Uri(value, UriKind.Absolute);
        }

        // Falls konfiguriert:
        if (!string.IsNullOrWhiteSpace(configured))
        {
            // Absolute URL?
            if (Uri.TryCreate(configured, UriKind.Absolute, out var abs))
                return MakeAbsolute(abs.AbsoluteUri);

            // Relativ (beginnt mit '/'): an Origin anhängen
            if (configured.StartsWith('/'))
            {
                var origin = new Uri(hostEnvironmentBaseAddress);
                return new Uri(origin, EnsureTrailingSlash(configured.TrimStart('/')));
            }

            // Relativ ohne '/': ebenfalls an Origin
            {
                var origin = new Uri(hostEnvironmentBaseAddress);
                return new Uri(origin, EnsureTrailingSlash(configured));
            }
        }

        // Standard ohne Konfiguration
        if (isDev)
        {
            // Lokales Dev-API (anpassen falls anderer Port)
            return MakeAbsolute("https://localhost:5005/api/");
        }

        // Produktion: gleiche Origin + /api/
        var siteOrigin = new Uri(hostEnvironmentBaseAddress);
        return new Uri(siteOrigin, "api/");
    }
}