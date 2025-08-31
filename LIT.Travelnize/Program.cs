using LIT.Travelnize;
using LIT.Travelnize.Utils.Api;
using LIT.Travelnize.Utils.Auth;
using LIT.Travelnize.Utils.Helpers;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        var backendBaseUrl = builder.Configuration["Backend:BaseUrl"]!;
        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(backendBaseUrl) });

        builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();
        builder.Services.AddAuthorizationCore();
        builder.Services.AddScoped<ApiClient>();
        builder.Services.AddScoped<AuthApiClient>();
        builder.Services.AddScoped<TripsApiClient>();
        builder.Services.AddScoped<LocalStorage>();

        await builder.Build().RunAsync();
    }
}