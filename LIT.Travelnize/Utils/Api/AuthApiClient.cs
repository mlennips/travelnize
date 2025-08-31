using LIT.Travelnize.Shared.Auth;
using LIT.Travelnize.Utils.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;

namespace LIT.Travelnize.Utils.Api
{
    public class AuthApiClient(HttpClient httpClient)
    {
        public async Task<HttpResponseMessage> RegisterAsync(RegisterCommand command)
        {
            return await httpClient.PostAsJsonAsync("/auth/register", command);
        }

        public async Task<LoginDto?> LoginAsync(string emailOrUserName, string password)
        {
            var command = new LoginCommand()
            {
                EmailOrUserName = emailOrUserName,
                Password = password
            };
            var response = await httpClient.PostAsJsonAsync("/auth/login", command);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginDto>();
                return result;
            }
            return null;
        }

        public async Task<LoginDto?> RefreshTokenAsync(RefreshCommand command)
        {
            var response = await httpClient.PostAsJsonAsync("/auth/refresh", command);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginDto>();
                return result;
            }
            return null;
        }

        public async Task<HttpResponseMessage> LogoutAsync()
        {
            return await httpClient.PostAsync("/auth/logout", null);
        }

        public async Task<HttpResponseMessage> ForgotPasswordAsync()
        {
            return await httpClient.PostAsync("/auth/forgot-password", null);
        }
    }
}
