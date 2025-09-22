namespace LIT.Travelnize.Domain.Trips.Commands
{
    public record AddGuestParticipantCommand(
        Guid TripId,
        string Name,
        Email Email
    ) : ICommand<Guid>;
}