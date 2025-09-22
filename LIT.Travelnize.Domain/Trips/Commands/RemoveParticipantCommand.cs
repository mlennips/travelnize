namespace LIT.Travelnize.Domain.Trips.Commands
{
    public record RemoveParticipantCommand(
        Guid TripId,
        Guid ParticipantId
    ) : ICommand;
}