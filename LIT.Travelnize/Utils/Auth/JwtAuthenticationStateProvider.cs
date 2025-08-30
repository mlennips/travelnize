using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Security.Claims;
using System.Text.Json;

namespace LIT.Travelnize.Utils.Auth
{
    public class JwtAuthenticationStateProvider(IJSRuntime jsRuntime) : AuthenticationStateProvider
    {
        private const string JwtKey = "auth_jwt";
        private const string AuthKey = "auth";
        private ClaimsIdentity? _identity;

        public async Task SetIdentityAsync(string token, Guid userId, string userName)
        {
            _identity = CreateClaimsIdentity(token, userId, userName);
            await SaveInLocalStorageAsync(token, userId, userName);

            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            if (_identity == null)
            {
                AuthData? auth = await LoadFromLocalStorageAsync();
                if (auth != null)
                {
                    _identity = CreateClaimsIdentity(auth.Token, Guid.Parse(auth.UserId), auth.UserName);
                }
            }
            var user = new ClaimsPrincipal(_identity ?? new ClaimsIdentity());
            return new AuthenticationState(user);
        }

        public async Task LogoutAsync()
        {
            _identity = null;
            await jsRuntime.InvokeVoidAsync("localStorage.removeItem", JwtKey);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        private async Task SaveInLocalStorageAsync(string token, Guid userId, string userName)
        {
            var authJson = JsonSerializer.Serialize(new AuthData(token, userId.ToString(), userName));
            await jsRuntime.InvokeVoidAsync("localStorage.setItem", AuthKey, authJson);
        }

        private async Task<AuthData?> LoadFromLocalStorageAsync()
        {
            var json = await jsRuntime.InvokeAsync<string>("localStorage.getItem", AuthKey);
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }
            return JsonSerializer.Deserialize<AuthData>(json);
        }

        private static ClaimsIdentity CreateClaimsIdentity(string token, Guid userId, string userName)
        {
            return new ClaimsIdentity(
            [
                new Claim(JwtKey, token),
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, userName)
            ], "jwt");
        }

        private record AuthData(string Token, string UserId, string UserName);
    }
}