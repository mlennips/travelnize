using LIT.Travelnize.Interfaces;
using System.Net.Http.Headers;

namespace LIT.Travelnize.Services.Auth
{
    public class JwtAuthorizationMessageHandler(IAccessTokenService accessTokenService) : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await accessTokenService.GetTokenAsync();
            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
