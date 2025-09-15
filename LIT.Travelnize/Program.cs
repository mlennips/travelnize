using Fluxor;
using LIT.Travelnize;
using LIT.Travelnize.Interfaces;
using LIT.Travelnize.Services;
using LIT.Travelnize.Services.Api;
using LIT.Travelnize.Services.Auth;
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

        builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();
        builder.Services.AddScoped<IAuthService, JwtAuthenticationStateProvider>();
        builder.Services.AddScoped<IAccessTokenService, AccessTokenService>();

        var backendBaseUrl = builder.Configuration["Backend:BaseUrl"]!;
        builder.Services.AddHttpClient<ApiClient>(string.Empty, client => { client.BaseAddress = new Uri(backendBaseUrl); });
        builder.Services.AddHttpClient<AuthApiClient>(string.Empty, client => { client.BaseAddress = new Uri(backendBaseUrl); });
        builder.Services.AddScoped<JwtAuthorizationMessageHandler>();
        builder.Services.AddHttpClient<TripsApiClient>(client =>
        {
            client.BaseAddress = new Uri(backendBaseUrl);
        })
        .AddHttpMessageHandler<JwtAuthorizationMessageHandler>();

        builder.Services.AddAuthorizationCore();
        builder.Services.AddScoped<LocalStorageService>();
        builder.Services.AddMudServices();

        builder.Services.AddFluxor(options => options
          .UseRouting()
          .ScanAssemblies(typeof(Program).Assembly));

        
        await builder.Build().RunAsync();
    }
}