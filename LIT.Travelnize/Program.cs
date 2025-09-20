using LIT.Travelnize;
using LIT.Travelnize.Interfaces;
using LIT.Travelnize.Services;
using LIT.Travelnize.Services.Api;
using LIT.Travelnize.Services.Auth;
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
        builder.Services.AddScoped<AuthenticationStateProvider>((p) => (JwtAuthenticationStateProvider)p.GetRequiredService<IAuthService>());
        builder.Services.AddScoped<JwtAuthorizationMessageHandler>();
        builder.Services.AddScoped<IAccessTokenService, AccessTokenService>();

        var backendBaseUrl = builder.Configuration["Backend:BaseUrl"]!;
        builder.Services.AddHttpClient<ApiClient>(string.Empty, client => { client.BaseAddress = new Uri(backendBaseUrl); });
        builder.Services.AddHttpClient<AuthApiClient>(string.Empty, client => { client.BaseAddress = new Uri(backendBaseUrl); });
        builder.Services.AddHttpClient<TripsApiClient>(client =>
        {
            client.BaseAddress = new Uri(backendBaseUrl);
        })
        .AddHttpMessageHandler<JwtAuthorizationMessageHandler>();

        builder.Services.AddAuthorizationCore();
        builder.Services.AddScoped<LocalStorageService>();
        builder.Services.AddMudServices();

        builder.Services.AddSingleton<TripsState>();

        await builder.Build().RunAsync();
    }
}