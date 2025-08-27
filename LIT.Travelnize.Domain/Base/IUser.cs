using LIT.Travelnize.Domain.Common;

namespace LIT.Travelnize.Domain.Base
{
    public interface IUser
    {
        public Guid Id { get; }
        public string Name { get; }
        public Email Email { get; }
    }
}
