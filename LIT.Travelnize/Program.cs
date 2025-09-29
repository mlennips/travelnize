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

        builder.Services.AddScoped<IAuthService, JwtAuthenticationStateProvider>();
        builder.Services.AddScoped<AuthenticationStateProvider>(p => (JwtAuthenticationStateProvider)p.GetRequiredService<IAuthService>());
        builder.Services.AddScoped<JwtAuthorizationMessageHandler>();
        builder.Services.AddScoped<IAccessTokenService, AccessTokenService>();

        // Backend Base URL robust ermitteln
        var backendBaseUrl = builder.Configuration["Backend:BaseUrl"];
        if (string.IsNullOrWhiteSpace(backendBaseUrl))
        {
            // Fallback: Development Default oder Fehler
            if (builder.HostEnvironment.IsDevelopment())
            {
                backendBaseUrl = "http://localhost:5005";
            }
            else
            {
                throw new InvalidOperationException("Backend:BaseUrl ist nicht konfiguriert (appsettings.json / appsettings.<Environment>.json).");
            }
        }

        var backendUri = new Uri(backendBaseUrl);

        builder.Services.AddHttpClient<ApiClient>(client => client.BaseAddress = backendUri);
        builder.Services.AddHttpClient<AuthApiClient>(client => client.BaseAddress = backendUri);
        builder.Services.AddHttpClient<TripsApiClient>(client => client.BaseAddress = backendUri)
            .AddHttpMessageHandler<JwtAuthorizationMessageHandler>();

        builder.Services.AddAuthorizationCore();
        builder.Services.AddScoped<LocalStorageService>();
        builder.Services.AddMudServices();
        builder.Services.AddScoped<ScrollService>();
        builder.Services.AddScoped<IWikipediaApiClient, WikipediaApiClient>();
        builder.Services.AddSingleton<TripsState>();

        await builder.Build().RunAsync();
    }
}