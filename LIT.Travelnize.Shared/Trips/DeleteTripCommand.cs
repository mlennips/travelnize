namespace LIT.Travelnize.Shared.Trips
{
    public record DeleteTripCommand(Guid TripId) : ICommand<Guid>;
}