using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LIT.Travelnize.Infrastructure.Identity
{
    public class IdentityDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public IdentityDbContext()
        {

        }

        public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
        {

        }
    }
}
