namespace LIT.Travelnize.Domain.Trips.Commands
{
    public record CreateTripCommand(
        string Name, 
        string Description, 
        string? DefaultTravelSegmentTitle,
        string? DefaultDestinationTitle) : ICommand<Guid>;
}
