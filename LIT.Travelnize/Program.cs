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

        // Einmalige Auflösung der Backend BaseAddress
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

        Console.WriteLine($"[BackendBase] final(once) = {resolvedBackendBase}");

        await builder.Build().RunAsync();
    }

    private static Uri ResolveBackendBaseAddress(WebAssemblyHostConfiguration config, string hostEnvironmentBaseAddress, bool isDev)
    {
        var raw = config["Backend:BaseUrl"];
        if (string.IsNullOrWhiteSpace(raw))
            raw = isDev ? "http://localhost:5005/" : "/api/";
        raw = raw.Trim();

        static string EnsureTrailingSlash(string s) => s.EndsWith('/') ? s : s + "/";

        // Absolute?
        if (Uri.TryCreate(raw, UriKind.Absolute, out var abs))
        {
            abs = new Uri(EnsureTrailingSlash(abs.AbsoluteUri));
            if (abs.Scheme == Uri.UriSchemeFile) // FIX: Uri.UriSchemeFile statt Uri.UriScheme.File
            {
                abs = BuildFromOrigin(hostEnvironmentBaseAddress, "api/");
                Console.WriteLine($"[BackendBase] WARN absolute file:// korrigiert -> {abs}");
            }
            return abs;
        }

        // Relativ
        var origin = BuildFromOrigin(hostEnvironmentBaseAddress, raw.TrimStart('/'));
        origin = new Uri(EnsureTrailingSlash(origin.AbsoluteUri));
        if (origin.Scheme == Uri.UriSchemeFile) // FIX: Uri.UriSchemeFile statt Uri.UriScheme.File
        {
            var fixedUri = BuildFromOrigin("http://localhost:8081/", "api/");
            Console.WriteLine($"[BackendBase] WARN relative file:// korrigiert -> {fixedUri}");
            return fixedUri;
        }

        return origin;

        static Uri BuildFromOrigin(string baseAddress, string append)
        {
            if (!baseAddress.EndsWith('/'))
                baseAddress += "/";
            var u = new Uri(baseAddress);
            return new Uri(u, append);
        }
    }
}