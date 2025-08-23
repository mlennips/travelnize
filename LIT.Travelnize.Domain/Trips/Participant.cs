using LIT.Travelnize.Domain.Base;
using LIT.Travelnize.Domain.Common;

namespace LIT.Travelnize.Domain.Trips
{
    public class Participant(Guid id, Guid tripId, Guid? userId, string name, Email? email, 
        PermissionLevel permissionLevel) : IEntity
    {
        public Guid Id { get; } = id;
        public Guid TripId { get; } = tripId;
        public Guid? UserId { get; private set; } = userId;

        public string Name { get; private set; } = name;
        public Email? Email { get; private set; } = email;
        public PermissionLevel PermissionLevel { get; private set; } = permissionLevel;

        internal static Participant Create(Guid tripId, Guid userId, string name, Email? email, PermissionLevel permissionLevel)
        {
            return new Participant(Guid.NewGuid(), tripId, userId, name, email, permissionLevel);
        }

        internal static Participant Create(Guid tripId, string name, Email? email, PermissionLevel permissionLevel)
        {
            return new Participant(Guid.NewGuid(), tripId, null, name, email, permissionLevel);
        }

        internal Result Update(string name, Email email)
        {
            Name = name;
            Email = email;
            return Result.Success();
        }

        internal Result ChangePermissionLevel(PermissionLevel newLevel)
        {
            PermissionLevel = newLevel;
            return Result.Success();
        }
    }
}