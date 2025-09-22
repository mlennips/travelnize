namespace LIT.Travelnize.Domain.Trips.Commands
{
    public record AddParticipantCommand(
        Guid TripId,
        Guid UserId,
        string Name,
        Email Email
    ) : ICommand<Guid>;
}