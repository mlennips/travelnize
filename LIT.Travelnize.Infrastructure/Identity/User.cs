using LIT.Travelnize.Domain.Base;
using Microsoft.AspNetCore.Identity;

namespace LIT.Travelnize.Infrastructure.Identity
{
    public class User : IdentityUser<Guid>, IUser
    {
        public string FirstName { get; init; } = default!;

        public string LastName { get; init; } = default!;
    }
}
