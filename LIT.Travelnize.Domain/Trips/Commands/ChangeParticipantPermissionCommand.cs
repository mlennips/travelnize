namespace LIT.Travelnize.Domain.Trips.Commands
{
    public record ChangeParticipantPermissionCommand(
        Guid TripId,
        Guid ParticipantId,
        PermissionLevel PermissionLevel
    ) : ICommand;
}