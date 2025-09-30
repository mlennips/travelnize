using LIT.Travelnize.Domain.Auth;
using System.Net.Http.Json;

namespace LIT.Travelnize.Services.Api
{
    public class AuthApiClient(HttpClient httpClient)
    {
        public async Task<HttpResponseMessage> RegisterAsync(RegisterCommand command)
            => await httpClient.PostAsJsonAsync("auth/register", command);

        public async Task<LoginDto?> LoginAsync(string emailOrUserName, string password)
        {
            var command = new LoginCommand
            {
                EmailOrUserName = emailOrUserName,
                Password = password
            };
            var response = await httpClient.PostAsJsonAsync("auth/login", command);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<LoginDto>();
            return null;
        }

        public async Task<LoginDto?> RefreshTokenAsync(RefreshCommand command)
        {
            var response = await httpClient.PostAsJsonAsync("auth/refresh", command);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<LoginDto>();
            return null;
        }

        public async Task<HttpResponseMessage> LogoutAsync()
            => await httpClient.PostAsync("auth/logout", null);

        public async Task<HttpResponseMessage> ForgotPasswordAsync(ForgotPasswordCommand command)
            => await httpClient.PostAsJsonAsync("auth/forgot-password", command);
    }
}
