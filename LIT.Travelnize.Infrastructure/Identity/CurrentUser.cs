using LIT.Travelnize.Domain.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace LIT.Travelnize.Infrastructure.Identity
{
    public class CurrentUser : ICurrentUser
    {
        private readonly UserManager<User> _userManager;
        private IUser? _user;

        public string Name { get; init; }

        public CurrentUser(IHttpContextAccessor httpContextAccessor, UserManager<User> userManager)
        {
            var user = httpContextAccessor?.HttpContext?.User;
            Name = user?.Claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value ?? "?";
            _userManager = userManager;
        }

        public async Task<IUser> GetUserAsync()
        {
            if (_user == null)
            {
                var identityUser = await _userManager.FindByNameAsync(Name);
                _user = identityUser ?? throw new InvalidOperationException($"User with name {Name} not found.");
            }
            return _user;
        }
    }
}
