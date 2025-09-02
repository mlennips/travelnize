using LIT.Travelnize.Interfaces;

namespace LIT.Travelnize.Services.Auth
{
    public class AccessTokenService(LocalStorageService localStorage) : IAccessTokenService
    {
        public Task<string?> GetTokenAsync()
        {
            return localStorage.GetValueAsync<string>("authToken", true);
        }

        public Task RemoveTokenAsync()
        {
            return localStorage.RemoveValueAsync("authToken");
        }

        public Task SetTokenAsync(string token)
        {
            return localStorage.SetValueAsync("authToken", token, true);
        }
    }
}
