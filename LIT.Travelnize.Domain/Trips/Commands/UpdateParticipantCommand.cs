namespace LIT.Travelnize.Domain.Trips.Commands
{
    public record UpdateParticipantCommand(
        Guid TripId,
        Guid ParticipantId,
        string Name,
        Email Email
    ) : ICommand;
}