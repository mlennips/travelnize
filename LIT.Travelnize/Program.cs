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
            if (env.IsDevelopment())
                raw = "http://localhost:5005/";
            else
                raw = "/api/"; // Prod Fallback
        }

        // Normalisieren: trailing slash erzwingen
        static Uri EnsureTrailingSlash(Uri u) =>
            u.AbsoluteUri.EndsWith('/') ? u : new Uri(u.AbsoluteUri + "/");

        // Absolute URL?
        if (Uri.TryCreate(raw, UriKind.Absolute, out var abs))
            return EnsureTrailingSlash(abs);

        // Relativ → an Origin anhängen
        var baseUri = new Uri(nav.BaseUri); // endet bereits mit '/'
        var combined = new Uri(baseUri, raw.TrimStart('/'));
        return EnsureTrailingSlash(combined);
    }
}