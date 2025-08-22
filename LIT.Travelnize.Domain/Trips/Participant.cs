using LIT.Travelnize.Domain.Base;
using LIT.Travelnize.Domain.Common;

namespace LIT.Travelnize.Domain.Trips
{
    public class Participant(Guid id, Guid userId, string name, Email email, 
        PermissionLevel permissionLevel) : IEntity
    {
        public Guid Id { get; } = id;
        public Guid UserId { get; } = userId;
        public string Name { get; private set; } = name;
        public Email Email { get; private set; } = email;
        public PermissionLevel PermissionLevel { get; private set; } = permissionLevel;

        public static Participant Create(Guid userId, string name, Email email, PermissionLevel permissionLevel)
        {
            return new Participant(Guid.NewGuid(), userId, name, email, permissionLevel);
        }

        public void Update(string name, Email email, PermissionLevel permissionLevel)
        {
            Name = name;
            Email = email;
            PermissionLevel = permissionLevel;
        }
    }
}