using LIT.Travelnize.Domain.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace LIT.Travelnize.Infrastructure.Identity
{
    public class CurrentUser : ICurrentUser
    {
        private const string AnonymousNameKey = "?";
        private readonly UserManager<User> _userManager;
        private IUser? _user;

        public string Name { get; }
        public string? RequestPath { get; }
        public string? HttpMethod { get; }
        public string? IpAddress { get; }
        public string? UserAgent { get; }
        public string? TraceIdentifier { get; }
        public string? Referer { get; }

        public CurrentUser(IHttpContextAccessor httpContextAccessor, UserManager<User> userManager)
        {
            var httpContext = httpContextAccessor?.HttpContext;
            var user = httpContext?.User;

            Name = user?.FindFirstValue(ClaimTypes.NameIdentifier) ?? AnonymousNameKey;

            var request = httpContext?.Request;
            RequestPath = request?.Path.Value;
            HttpMethod = request?.Method;
            IpAddress = httpContext?.Connection?.RemoteIpAddress?.ToString();
            UserAgent = request?.Headers.UserAgent.ToString();
            TraceIdentifier = httpContext?.TraceIdentifier;
            Referer = request?.Headers.Referer.ToString();

            _userManager = userManager;
        }

        public async Task<IUser> GetUserAsync()
        {
            if (_user is not null)
                return _user;

            if (Name == AnonymousNameKey)
                return _user = new User { Id = Guid.Empty, UserName = "Anonymous" };

            var identityUser = await _userManager.FindByNameAsync(Name);
            return _user = identityUser ?? throw new InvalidOperationException($"User with name {Name} not found.");
        }
    }
}
