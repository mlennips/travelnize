using LIT.Travelnize.Domain.Auth;
using LIT.Travelnize.Interfaces;
using LIT.Travelnize.Services.Api;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace LIT.Travelnize.Services.Auth
{
    public class JwtAuthenticationStateProvider(
        LocalStorageService localStorage, 
        AuthApiClient authApiClient,
        IAccessTokenService accessTokenService) : AuthenticationStateProvider, IAuthService
    {
        private const string JwtKey = "jwt";
        private const string AuthKey = "auth";
        private ClaimsIdentity? _identity;
        private AuthData? _authData;

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            if (_identity == null)
            {
                await RefreshTokenAsync();
            }
            var user = new ClaimsPrincipal(_identity ?? new ClaimsIdentity());
            return new AuthenticationState(user);
        }

        public async Task<bool> LoginAsync(string emailOrUserName, string password)
        {
            var response = await authApiClient.LoginAsync(emailOrUserName, password);
            if (response != null)
            {
                await SetIdentityAsync(response.Token, response.UserId, response.UserName, response.FirstName, response.LastName);
                NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
                return true;
            }
            return false;
        }

        public async Task<bool> LogoutAsync()
        {
            _identity = null;
            _authData = null;
            await accessTokenService.RemoveTokenAsync();
            await localStorage.RemoveValueAsync(AuthKey);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
            return true;
        }

        public async Task<IAuthUser?> GetCurrentUserAsync()
        {
            var isAuth = await CheckIsAuthenticatedAsync();
            if (!isAuth)
            {
                return null;
            }
            return _authData;
        }

        public async Task<bool> CheckIsAuthenticatedAsync()
        {
            return (await GetAuthenticationStateAsync()).User.Identity?.IsAuthenticated ?? false;
        }

        private async Task<bool> RefreshTokenAsync()
        {
            var authData = await localStorage.GetValueAsync<AuthData>(AuthKey, true);
            if (authData == null)
            {
                return false;
            }
            var response = await authApiClient.RefreshTokenAsync(new RefreshCommand { Token = authData.Token, UserId = authData.UserId });
            if (response != null)
            {
                await SetIdentityAsync(response.Token, response.UserId, response.UserName, response.FirstName, response.LastName);
                return true;
            }
            return false;
        }

        private async Task SetIdentityAsync(string token, Guid userId, string userName, string firstName, string lastName)
        {
            _identity = CreateClaimsIdentity(token, userId, userName, firstName, lastName);
            _authData = new AuthData(token, userId, userName, firstName, lastName);
            await accessTokenService.SetTokenAsync(token);
            await localStorage.SetValueAsync(AuthKey, _authData, true);
        }

        private static ClaimsIdentity CreateClaimsIdentity(string token, Guid userId, string userName, string firstName, string lastName)
        {
            return new ClaimsIdentity(
            [
                new Claim(JwtKey, token),
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.GivenName, firstName),
                new Claim(ClaimTypes.Surname, lastName)
            ], JwtKey);
        }

        private record AuthData(string Token, Guid UserId, string UserName, string FirstName, string LastName) : IAuthUser;
    }
}