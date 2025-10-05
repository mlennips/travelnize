using System.ComponentModel.DataAnnotations.Schema;

namespace LIT.Travelnize.Domain.Trips
{
    public class Participant : IAuditableEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; init; }
        public Guid TripId { get; init; }
        public Guid AggregateId => TripId;

        public Guid? UserId { get; private set; }
        public string Name { get; private set; } = default!;
        public Email? Email { get; private set; }
        public PermissionLevel PermissionLevel { get; private set; } = PermissionLevel.Guest;

        internal static Participant CreateAsUser(Guid tripId, Guid userId, string name, Email? email)
        {
            return new Participant
            {
                Id = Guid.NewGuid(),
                TripId = tripId,
                UserId = userId,
                Name = name,
                Email = email,
                PermissionLevel = PermissionLevel.User
            };
        }

        internal static Participant CreateAsGuest(Guid tripId, string name, Email? email)
        {
            return new Participant
            {
                Id = Guid.NewGuid(),
                TripId = tripId,
                UserId = null,
                Name = name,
                Email = email,
                PermissionLevel = PermissionLevel.Guest
            };
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