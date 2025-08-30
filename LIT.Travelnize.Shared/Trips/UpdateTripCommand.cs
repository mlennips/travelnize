namespace LIT.Travelnize.Shared.Trips
{
    public record UpdateTripCommand(Guid TripId, string Name, string Description, DateTime TravelStart, DateTime TravelEnd) : ICommand;
}
