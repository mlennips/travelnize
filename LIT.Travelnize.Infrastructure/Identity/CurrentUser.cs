using LIT.Travelnize.Domain.Base;
using LIT.Travelnize.Domain.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace LIT.Travelnize.Infrastructure.Identity
{
    public class CurrentUser : ICurrentUser
    {
        private readonly UserManager<IdentityUser> _userManager;
        private IUser? _user;

        public string Name { get; init; }

        public CurrentUser(IHttpContextAccessor httpContextAccessor, UserManager<IdentityUser> userManager)
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
                _user = identityUser == null
                    ? throw new InvalidOperationException($"User with name {Name} not found.")
                    : new User(Guid.Parse(identityUser.Id), identityUser.UserName!, new Email(identityUser.Email!));
            }
            return _user;
        }

        private sealed class User(Guid id, string name, Email email) : IUser
        {
            public Guid Id { get; } = id;
            public string Name { get; } = name;
            public Email Email { get; } = email;
        }
    }
}
