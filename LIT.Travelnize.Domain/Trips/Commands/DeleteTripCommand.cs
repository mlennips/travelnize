namespace LIT.Travelnize.Domain.Trips.Commands
{
    public record DeleteTripCommand(Guid TripId) : ICommand<Guid>;
}