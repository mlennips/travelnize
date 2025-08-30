using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace LIT.Travelnize.Utils.Auth
{
    public class JwtAuthenticationStateProvider : AuthenticationStateProvider
    {
        private string? _token;

        public void SetToken(string? token)
        {
            _token = token;
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            ClaimsIdentity identity = string.IsNullOrEmpty(_token)
                ? new ClaimsIdentity()
                : new ClaimsIdentity([new Claim("jwt", _token)], "jwt");

            var user = new ClaimsPrincipal(identity);
            return Task.FromResult(new AuthenticationState(user));
        }
    }
}