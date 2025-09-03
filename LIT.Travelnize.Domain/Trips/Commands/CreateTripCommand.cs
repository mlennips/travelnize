namespace LIT.Travelnize.Domain.Trips.Commands
{
    public record CreateTripCommand(string Name, string Description, DateTime TravelStart, DateTime TravelEnd) : ICommand<Guid>;
}
