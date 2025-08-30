using LIT.Travelnize.Shared.Auth;
using LIT.Travelnize.Shared.Trips;
using LIT.Travelnize.Utils.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;

namespace LIT.Travelnize.Utils.Api
{
    public class ApiClient(HttpClient httpClient, AuthenticationStateProvider authenticationStateProvider)
    {
        public async Task<string> GetWelcomeAsync()
        {
            return await httpClient.GetStringAsync("/welcome");
        }

        // Auth Endpunkte

        public async Task<HttpResponseMessage> RegisterAsync(RegisterCommand command)
        {
            return await httpClient.PostAsJsonAsync("/auth/register", command);
        }

        public async Task<bool> LoginAsync(LoginCommand command)
        {
            var response = await httpClient.PostAsJsonAsync("/auth/login", command);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginDto>();
                if(result != null)
                {
                    if (authenticationStateProvider is JwtAuthenticationStateProvider jwtAuthenticationStateProvider)
                    {
                        await jwtAuthenticationStateProvider.SetIdentityAsync(result.Token, result.UserId, result.Name);
                        return true;
                    }
                    else
                    {
                        throw new InvalidOperationException("AuthenticationStateProvider is not of type JwtAuthenticationStateProvider");
                    }
                }
                return true;
            }
            return false;
        }

        public async Task<HttpResponseMessage> LogoutAsync()
        {
            return await httpClient.PostAsync("/auth/logout", null);
        }

        public async Task<HttpResponseMessage> ForgotPasswordAsync()
        {
            return await httpClient.PostAsync("/auth/forgot-password", null);
        }

        // Trips Endpunkte

        public async Task<Guid?> CreateTripAsync(CreateTripCommand command)
        {
            var response = await httpClient.PostAsJsonAsync("/trips", command);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Guid>();
            }
            return null;
        }

        public async Task<ListTripsDto[]?> GetTripsAsync(Guid userId)
        {
            return await httpClient.GetFromJsonAsync<ListTripsDto[]>($"/trips?userId={userId}");
        }

        public async Task<GetTripDto?> GetTripAsync(Guid tripId)
        {
            return await httpClient.GetFromJsonAsync<GetTripDto>($"/trips/{tripId}");
        }

        public async Task<bool> UpdateTripAsync(Guid tripId, UpdateTripCommand command)
        {
            var response = await httpClient.PutAsJsonAsync($"/trips/{tripId}", command);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteTripAsync(Guid tripId)
        {
            var response = await httpClient.DeleteAsync($"/trips/{tripId}");
            return response.IsSuccessStatusCode;
        }
    }
}
