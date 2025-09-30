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
        var configured = config["Backend:BaseUrl"]?.Trim();
        string EnsureSlash(string v) => v.EndsWith('/') ? v : v + "/";

        Uri MakeAbsolute(string value) => new(EnsureSlash(value), UriKind.Absolute);

        // Helper: Origin
        var origin = new Uri(hostEnvironmentBaseAddress);

        // 1. Konfig explizit?
        if (!string.IsNullOrWhiteSpace(configured))
        {
            // Absolute?
            if (Uri.TryCreate(configured, UriKind.Absolute, out var abs))
            {
                // Schutz: interne / Docker-only Hosts (kein Punkt) in Produktion ignorieren
                if (!isDev && abs.Host.IndexOf('.') < 0)
                {
                    var corrected = new Uri(origin, "api/");
                    Console.WriteLine($"[BackendBase] WARN ignoring internal host '{abs.Host}' in production -> {corrected}");
                    return corrected;
                }

                return MakeAbsolute(abs.AbsoluteUri);
            }

            // Relativ mit führendem /
            if (configured.StartsWith('/'))
                return new Uri(origin, EnsureSlash(configured.TrimStart('/')));

            // Relativ ohne /
            return new Uri(origin, EnsureSlash(configured));
        }

        // 2. Fallbacks
        if (isDev)
            return MakeAbsolute("https://localhost:5005/api/");

        // Produktion: gleiche Origin + /api/
        return new Uri(origin, "api/");
    }
}