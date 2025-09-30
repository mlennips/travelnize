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

        builder.Services.AddSingleton(s =>
        {
            return new BackendBaseAddressResolver(
                builder.Configuration, 
                builder.HostEnvironment.BaseAddress, 
                builder.HostEnvironment.IsDevelopment());
        });
        builder.Services.AddHttpClient<ApiClient>((sp, c) =>
        {
            c.BaseAddress = sp.GetRequiredService<BackendBaseAddressResolver>().BaseAddress;
        });
        builder.Services.AddHttpClient<AuthApiClient>((sp, c) =>
        {
            c.BaseAddress = sp.GetRequiredService<BackendBaseAddressResolver>().BaseAddress;
        });
        builder.Services.AddHttpClient<TripsApiClient>((sp, c) =>
        {
            c.BaseAddress = sp.GetRequiredService<BackendBaseAddressResolver>().BaseAddress;
        }).AddHttpMessageHandler<JwtAuthorizationMessageHandler>();
        builder.Services.AddHttpClient<IWikipediaApiClient, WikipediaApiClient>((sp, c) =>
        {
            c.BaseAddress = sp.GetRequiredService<BackendBaseAddressResolver>().BaseAddress;
        });

        builder.Services.AddAuthorizationCore();
        builder.Services.AddScoped<LocalStorageService>();
        builder.Services.AddMudServices();
        builder.Services.AddScoped<ScrollService>();
        builder.Services.AddSingleton<TripsState>();

        await builder.Build().RunAsync();
    }

}