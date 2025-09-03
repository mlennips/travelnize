namespace LIT.Travelnize.Domain.Trips.Commands
{
    public record UpdateTripCommand(Guid TripId, string Name, string Description, DateTime TravelStart, DateTime TravelEnd) : ICommand;
}
