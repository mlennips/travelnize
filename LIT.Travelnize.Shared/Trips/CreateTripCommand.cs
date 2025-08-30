namespace LIT.Travelnize.Shared.Trips
{
    public record CreateTripCommand(string Name, string Description, DateTime TravelStart, DateTime TravelEnd) : ICommand<Guid>;
}
